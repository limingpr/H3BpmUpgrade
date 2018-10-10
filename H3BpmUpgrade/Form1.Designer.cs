namespace H3BpmUpgrade
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnDb = new System.Windows.Forms.Button();
            this.btnSyncOrg = new System.Windows.Forms.Button();
            this.btnBizService = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnWorkflow = new System.Windows.Forms.Button();
            this.btnObjectData = new System.Windows.Forms.Button();
            this.btnPara = new System.Windows.Forms.Button();
            this.btnApp = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnDb
            // 
            this.btnDb.Location = new System.Drawing.Point(41, 56);
            this.btnDb.Name = "btnDb";
            this.btnDb.Size = new System.Drawing.Size(200, 40);
            this.btnDb.TabIndex = 0;
            this.btnDb.Text = "生成数据库脚本";
            this.btnDb.UseVisualStyleBackColor = true;
            this.btnDb.Click += new System.EventHandler(this.btnDb_Click);
            // 
            // btnSyncOrg
            // 
            this.btnSyncOrg.Location = new System.Drawing.Point(41, 150);
            this.btnSyncOrg.Name = "btnSyncOrg";
            this.btnSyncOrg.Size = new System.Drawing.Size(200, 40);
            this.btnSyncOrg.TabIndex = 1;
            this.btnSyncOrg.Text = "组织架构";
            this.btnSyncOrg.UseVisualStyleBackColor = true;
            this.btnSyncOrg.Click += new System.EventHandler(this.btnSyncOrg_Click);
            // 
            // btnBizService
            // 
            this.btnBizService.Location = new System.Drawing.Point(347, 150);
            this.btnBizService.Name = "btnBizService";
            this.btnBizService.Size = new System.Drawing.Size(200, 40);
            this.btnBizService.TabIndex = 2;
            this.btnBizService.Text = "业务服务与规则";
            this.btnBizService.UseVisualStyleBackColor = true;
            this.btnBizService.Click += new System.EventHandler(this.btnBizService_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(347, 56);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(200, 40);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "重启服务";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnWorkflow
            // 
            this.btnWorkflow.Location = new System.Drawing.Point(651, 150);
            this.btnWorkflow.Name = "btnWorkflow";
            this.btnWorkflow.Size = new System.Drawing.Size(200, 40);
            this.btnWorkflow.TabIndex = 4;
            this.btnWorkflow.Text = "流程模型";
            this.btnWorkflow.UseVisualStyleBackColor = true;
            this.btnWorkflow.Click += new System.EventHandler(this.btnWorkflow_Click);
            // 
            // btnObjectData
            // 
            this.btnObjectData.Location = new System.Drawing.Point(41, 236);
            this.btnObjectData.Name = "btnObjectData";
            this.btnObjectData.Size = new System.Drawing.Size(200, 40);
            this.btnObjectData.TabIndex = 5;
            this.btnObjectData.Text = "流程数据";
            this.btnObjectData.UseVisualStyleBackColor = true;
            this.btnObjectData.Click += new System.EventHandler(this.btnObjectData_Click);
            // 
            // btnPara
            // 
            this.btnPara.Location = new System.Drawing.Point(651, 236);
            this.btnPara.Name = "btnPara";
            this.btnPara.Size = new System.Drawing.Size(200, 40);
            this.btnPara.TabIndex = 6;
            this.btnPara.Text = "参数设置";
            this.btnPara.UseVisualStyleBackColor = true;
            this.btnPara.Click += new System.EventHandler(this.btnPara_Click);
            // 
            // btnApp
            // 
            this.btnApp.Location = new System.Drawing.Point(347, 236);
            this.btnApp.Name = "btnApp";
            this.btnApp.Size = new System.Drawing.Size(200, 40);
            this.btnApp.TabIndex = 7;
            this.btnApp.Text = "应用中心";
            this.btnApp.UseVisualStyleBackColor = true;
            this.btnApp.Click += new System.EventHandler(this.btnApp_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(974, 629);
            this.Controls.Add(this.btnApp);
            this.Controls.Add(this.btnPara);
            this.Controls.Add(this.btnObjectData);
            this.Controls.Add(this.btnWorkflow);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnBizService);
            this.Controls.Add(this.btnSyncOrg);
            this.Controls.Add(this.btnDb);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnDb;
        private System.Windows.Forms.Button btnSyncOrg;
        private System.Windows.Forms.Button btnBizService;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnWorkflow;
        private System.Windows.Forms.Button btnObjectData;
        private System.Windows.Forms.Button btnPara;
        private System.Windows.Forms.Button btnApp;
    }
}

