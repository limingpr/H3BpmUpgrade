using H3BpmUpgrade.Business;
using H3BpmUpgrade.Helper;
using OThinker.H3.WorkflowTemplate;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace H3BpmUpgrade
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnDb_Click(object sender, EventArgs e)
        {
            CreateSystemTable();
            //CreateCustomerTable();
            //OT_WorkflowClause WorkflowName-->DisplayName
            MessageBox.Show("创建数据库脚本成功");
        }

        private void CreateCustomerTable()
        {
            var sqltable = string.Format(@"SELECT
	*
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
AND Table_Name  LIKE 'I_%'");

            var dt = H3DBHelper.GetDataTable(sqltable);
            foreach (DataRow item in dt.Rows)
            {
                var TableName = item["TABLE_NAME"].ToString();

                var temp = DataBusiness.GetTableCols(TableName);

                DataBusiness.CreateTable(temp);
                DataBusiness.CreateType(temp);
                DataBusiness.CreateProc(temp);

            }

        }

        private void CreateSystemTable()
        {
            var sqltable = string.Format(@"SELECT
	*
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
AND Table_Name  LIKE 'OT_%'");

            var dt = H3DBHelper.GetDataTable(sqltable);
            foreach (DataRow item in dt.Rows)
            {
                CreateDBScript(item["TABLE_NAME"].ToString());
            }

        }

        private void CreateDBScript(string TableName)
        {
            var temp = DataBusiness.GetTableCols(TableName);
            DataBusiness.CreateType(temp);
            DataBusiness.CreateProc(temp);
            LogHelper.Info("创建脚本：" + TableName);
        }

        private void btnSyncOrg_Click(object sender, EventArgs e)
        {
            //组织类型
            DataBusiness.SyncTable(new string[] { "OT_OrgCategory", "OT_Category" });
            //OU表
            DataBusiness.SyncTable("OT_OrganizationUnit");
            //用户
            DataBusiness.SyncTable("OT_User");

            //组
            DataBusiness.SyncTable("OT_Group");

            //组成员  
            DataBusiness.SyncTable("OT_GroupChild");
            //重启引擎
            StartService("H3SharedService");

            //MessageBox.Show("组织架构导入完成，请重启引擎");
            SyncRole();


        }

        /// <summary>
        /// 角色同步
        /// </summary>
        private void SyncRole()
        {
            //角色 
            //说明V10版本中的角色相当于V9版本中职务、岗位、编制的集合。所以在同步时，要进行转化
            //岗位名称->角色名称
            //岗位编码->角色编码
            //岗位成员->角色用户
            //岗位所在部门->角色管理范围

            var sqlorgjob = string.Format(@"SELECT
	[ObjectID]
   ,[Code]
   ,[SuperiorCode]
   ,[DisplayName]
   ,[Description]
   ,[ParentObjectID]
   ,[ParentPropertyName]
   ,[ParentIndex]
   ,[Level]
FROM [OT_OrgJob]
ORDER BY Code");
            var dtjob = H3DBHelper.GetDataTable(sqlorgjob);
            foreach (DataRow item in dtjob.Rows)
            {
                var orgpost = new OThinker.Organization.OrgPost
                {
                    ObjectID = item["ObjectID"].ToString(),
                    Code = item["Code"].ToString(),
                    Name = item["DisplayName"].ToString(),
                    JobLevel = item["Level"].ToString() == "" ? 0 : int.Parse(item["Level"].ToString())

                };

                var staff = string.Format(@"SELECT
	                                            t3.ChildID
                                               ,t2.ParentID
                                               ,t1.Code
                                            FROM OT_OrgJob t1
	                                            ,OT_OrgPost t2
	                                            ,OT_GroupChild t3
                                            WHERE t1.Code = t2.JobCode
                                            AND t2.ObjectID = t3.ParentObjectID
                                            AND t1.Code='{0}'
                                            ORDER BY t1.Code, t2.Code, t2.ParentID", orgpost.Code);

                var dtorgstaff = H3DBHelper.GetDataTable(staff);
                var list = new List<OThinker.Organization.OrgStaff>();
                foreach (DataRow item2 in dtorgstaff.Rows)
                {
                    var orgstaff = new OThinker.Organization.OrgStaff
                    {
                        OUScope = new string[] { item2["ParentID"].ToString() },
                        UserID = item2["ChildID"].ToString(),
                        ParentObjectID = orgpost.ObjectID

                    };
                    list.Add(orgstaff);
                }
                orgpost.ChildList = list.ToArray();
                OThinker.H3.Controllers.AppUtility.Engine.Organization.AddUnit("", orgpost);

            }


            //编制名称->角色名称
            //编制编码->角色编码
            //编制成员->角色用户
            //编制管理部门->角色管理范围

            //组
            var sqlgroup = string.Format(@"SELECT
	                                            ObjectID
                                               ,Name
                                               ,Code
                                               ,ParentID
                                            FROM [OT_Group]");
            var dtgroup = H3DBHelper.GetDataTable(sqlgroup);
            foreach (DataRow item in dtgroup.Rows)
            {
                var group = new OThinker.Organization.Group()
                {
                    ObjectID = item["ObjectID"].ToString(),
                    Name = item["Name"].ToString(),
                    ParentID = item["ParentID"].ToString()
                };
                var groupchild = string.Format(@"SELECT
	                                                *
                                                FROM OT_GroupChild
                                                WHERE ParentObjectID = '{0}'", item["ObjectID"].ToString());
                var dtchild = H3DBHelper.GetDataTable(groupchild);
                foreach (DataRow item2 in dtchild.Rows)
                {
                    var list = new List<OThinker.Organization.GroupChild>();
                    var staff = new OThinker.Organization.GroupChild
                    {
                        ObjectID = item2["ObjectID"].ToString(),
                        ChildID = item2["ChildID"].ToString(),
                        ParentObjectID = item2["ParentObjectID"].ToString()
                    };
                    list.Add(staff);
                    group.ChildList = list.ToArray();

                }

                OThinker.H3.Controllers.AppUtility.Engine.Organization.AddUnit("", group);


            }

        }


        private static void StartService(string serviceName)
        {
            Process process;
            try
            {
                StopWindowsService(serviceName);
                ProcessStartInfo info = new ProcessStartInfo("NET", "Start " + serviceName);
                info.CreateNoWindow = false;
                info.UseShellExecute = false;
                info.ErrorDialog = true;

                process = Process.Start(info);

                LogHelper.Info(string.Format("服务启动成功:{0}......", serviceName));
            }
            catch (Exception e)
            {
                LogHelper.Error(string.Format(@"服务启动失败，服务名称：{0}，错误：{1}", serviceName, e.Message));

            }

        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="windowsServiceName">服务名称</param>
        static void StopWindowsService(string windowsServiceName)
        {
            using (System.ServiceProcess.ServiceController control = new System.ServiceProcess.ServiceController(windowsServiceName))
            {
                if (control.Status == System.ServiceProcess.ServiceControllerStatus.Running)
                {
                    Console.WriteLine("服务停止......");
                    control.Stop();
                    Console.WriteLine("服务已经停止......");
                }
                else if (control.Status == System.ServiceProcess.ServiceControllerStatus.Stopped)
                {
                    Console.WriteLine("服务已经停止......");
                }
            }
        }

        private void btnBizService_Click(object sender, EventArgs e)
        {
            //业务数据库的链接
            DataBusiness.SyncTable("OT_BizDbConnectionConfig");
            //业务服务
            DataBusiness.SyncTable("OT_BizService");
            DataBusiness.SyncTable("OT_BizServiceMethod");
            DataBusiness.SyncTable("OT_BizServiceSetting");
            //业务规则
            DataBusiness.SyncTable("OT_BizRule");
            DataBusiness.SyncTable("OT_BizRuleAcl");
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            StartService("H3SharedService");

        }

        private void btnWorkflow_Click(object sender, EventArgs e)
        {
            //数据模型
            DataBusiness.SyncTable("OT_BizObjectSchemaDraft");
            DataBusiness.SyncTable("OT_BizObjectSchemaPublished");
            DataBusiness.SyncTable("OT_BizObjectAcl");
            DataBusiness.SyncTable("OT_BizQuery");
            DataBusiness.SyncTable("OT_BizQueryAction");
            DataBusiness.SyncTable("OT_BizQueryColumn");
            DataBusiness.SyncTable("OT_BizQueryItems");
            DataBusiness.SyncTable("OT_BizListener");
            //流程表单
            DataBusiness.SyncTable("OT_BizSheet");

            //流程图
            DataBusiness.SyncTable("OT_WorkflowClause");
            DataBusiness.SyncTable("OT_WorkflowAcl");
            DataBusiness.SyncTable("OT_WorkflowClauseSeqNo");
            DataBusiness.SyncTable("OT_WorkflowTemplateDraft");
            DataBusiness.SyncTable("OT_WorkflowTemplatePublished");
            DataBusiness.SyncTable("OT_ActivityConfig");

            //流程测试用例
            DataBusiness.SyncTable("OT_InstanceSimulation");
            DataBusiness.SyncTable("OT_InstanceSimulationDataItem");
            DataBusiness.SyncTable("OT_InstanceSimulationList");
            DataBusiness.SyncTable("OT_InstanceSimulationLog");

            //重启引擎
            StartService("H3SharedService");



        }

        private void btnObjectData_Click(object sender, EventArgs e)
        {
            DataBusiness.SyncTable("OT_InstanceContext");

            DataBusiness.SyncTable("OT_WorkItem");

            DataBusiness.SyncTable("OT_Comment");

            DataBusiness.SyncTable("OT_Timer");

            DataBusiness.SyncTable("OT_Token");

            DataBusiness.SyncTable("OT_Urgency");

            //

        }

        private void btnPara_Click(object sender, EventArgs e)
        {
            //常用流程
            DataBusiness.SyncTable("OT_FavoriteWorkflow");
            //
            DataBusiness.SyncTable("OT_FrequentlyUsedComment");

            DataBusiness.SyncTable("OT_Signature");

            //
            DataBusiness.SyncTable("OT_Agency");
            //
            DataBusiness.SyncTable("OT_Consultancy");

            //系统管理员
            DataBusiness.SyncTable("OT_SystemAcl");
            //组织权限
            DataBusiness.SyncTable("OT_SystemOrgAcl");
            //文件储存
            DataBusiness.SyncTable("OT_FileServer");
            //工作日历
            DataBusiness.SyncTable("OT_WorkingCalendar");
            DataBusiness.SyncTable("OT_WorkingDay");
            DataBusiness.SyncTable("OT_WorkingTimeSpan");

            //数据字典
            DataBusiness.SyncTable("OT_EnumerableMetadata");


        }

        private void btnApp_Click(object sender, EventArgs e)
        {
            DataBusiness.SyncTable("OT_AppNavigation");
            var WorkflowClauses = OThinker.H3.Controllers.AppUtility.Engine.Query.QueryClause(null);
            //foreach (string item in WorkflowClauses)
            //{

            //    UpdateWorkflow(item);
            //}
            UpdateWorkflow("SAPJurisdictionChange");

        }

        private void UpdateWorkflow(string WorkflowCode)
        {
            var DraftTemplate = OThinker.H3.Controllers.AppUtility.Engine.WorkflowManager.GetDraftTemplate(WorkflowCode);
            foreach (var item2 in DraftTemplate.Activities)
            {
                if (item2.ActivityType == OThinker.H3.WorkflowTemplate.ActivityType.Approve)
                {
                    var ApproveActivity1 = item2 as ApproveActivity;
                    ApproveActivity1.Participants = ApproveActivity1.Participants.Replace("FindPostByJobCode", "FindPostByCode").Replace("FindPostByJobCode", "FindPostByCode");

                }
            }
            OThinker.H3.Controllers.AppUtility.Engine.WorkflowManager.SaveDraftTemplate("", DraftTemplate);
            //var PublishedTemplate = OThinker.H3.Controllers.AppUtility.Engine.WorkflowManager.GetPublishedTemplateHeaders(WorkflowCode);

            //foreach (var item2 in PublishedTemplate.Activities)
            //{
            //    if (item2.ActivityType == OThinker.H3.WorkflowTemplate.ActivityType.Approve)
            //    {
            //        var ApproveActivity1 = item2 as ApproveActivity;
            //        ApproveActivity1.Participants = ApproveActivity1.Participants.Replace("FindPostByJobCode", "FindPostByCode").Replace("FindPostByJobCode", "FindPostByCode");

            //    }
            //    if (item2.ActivityType == OThinker.H3.WorkflowTemplate.ActivityType.FillSheet)
            //    {
            //        var ApproveActivity1 = item2 as FillSheetActivity;
            //        ApproveActivity1.Participants = ApproveActivity1.Participants.Replace("FindPostByJobCode", "FindPostByCode").Replace("FindPostByJobCode", "FindPostByCode");

            //    }
            //}
            OThinker.H3.Controllers.AppUtility.Engine.WorkflowManager.RegisterWorkflow("", DraftTemplate.BizObjectSchemaCode, true);

        }
    }
}
