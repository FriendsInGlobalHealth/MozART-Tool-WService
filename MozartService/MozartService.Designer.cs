
namespace MozartService
{
    partial class MozartService
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.eventLog1 = new System.Diagnostics.EventLog();
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            this.fileSystemWatcher2 = new System.IO.FileSystemWatcher();
            this.fileSystemWatcher3 = new System.IO.FileSystemWatcher();
            this.fileSystemWatcher4 = new System.IO.FileSystemWatcher();
            this.fileSystemWatcher5 = new System.IO.FileSystemWatcher();
            this.fileSystemWatcher6 = new System.IO.FileSystemWatcher();
            this.fileSystemWatcher7 = new System.IO.FileSystemWatcher();
            ((System.ComponentModel.ISupportInitialize)(this.eventLog1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher7)).BeginInit();
            // 
            // eventLog1
            // 
            this.eventLog1.EntryWritten += new System.Diagnostics.EntryWrittenEventHandler(this.eventLog1_EntryWritten);
            // 
            // fileSystemWatcher1
            // 
            this.fileSystemWatcher1.EnableRaisingEvents = true;
            this.fileSystemWatcher1.Path = "E:\\SFTPRoot\\Mozart Submissions\\FGH";
            this.fileSystemWatcher1.Created += new System.IO.FileSystemEventHandler(this.fileSystemWatcher1_Created);
            // 
            // fileSystemWatcher2
            // 
            this.fileSystemWatcher2.EnableRaisingEvents = true;
            this.fileSystemWatcher2.Path = "E:\\SFTPRoot\\Mozart Submissions\\ARIEL";
            this.fileSystemWatcher2.Created += new System.IO.FileSystemEventHandler(this.fileSystemWatcher2_Created);
            // 
            // fileSystemWatcher3
            // 
            this.fileSystemWatcher3.EnableRaisingEvents = true;
            this.fileSystemWatcher3.Path = "E:\\SFTPRoot\\Mozart Submissions\\CCS";
            this.fileSystemWatcher3.Created += new System.IO.FileSystemEventHandler(this.fileSystemWatcher3_Created);
            // 
            // fileSystemWatcher4
            // 
            this.fileSystemWatcher4.EnableRaisingEvents = true;
            this.fileSystemWatcher4.Path = "E:\\SFTPRoot\\Mozart Submissions\\ECHO";
            this.fileSystemWatcher4.Created += new System.IO.FileSystemEventHandler(this.fileSystemWatcher4_Created);
            // 
            // fileSystemWatcher5
            // 
            this.fileSystemWatcher5.EnableRaisingEvents = true;
            this.fileSystemWatcher5.Path = "E:\\SFTPRoot\\Mozart Submissions\\ICAP";
            this.fileSystemWatcher5.Created += new System.IO.FileSystemEventHandler(this.fileSystemWatcher5_Created);
            // 
            // fileSystemWatcher6
            // 
            this.fileSystemWatcher6.EnableRaisingEvents = true;
            this.fileSystemWatcher6.Path = "E:\\SFTPRoot\\Mozart Submissions\\EGPAF";
            this.fileSystemWatcher6.Created += new System.IO.FileSystemEventHandler(this.fileSystemWatcher6_Created);
            // 
            // fileSystemWatcher7
            // 
            this.fileSystemWatcher7.EnableRaisingEvents = true;
            this.fileSystemWatcher7.Path = "C:\\MOZART\\SQL_DUMPS";
            this.fileSystemWatcher7.Changed += new System.IO.FileSystemEventHandler(this.fileSystemWatcher7_Changed);
            this.fileSystemWatcher7.Created += new System.IO.FileSystemEventHandler(this.fileSystemWatcher7_Created);
            // 
            // MozartService
            // 
            this.ServiceName = "MozartService ";
            ((System.ComponentModel.ISupportInitialize)(this.eventLog1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher7)).EndInit();

        }

        #endregion

        private System.Diagnostics.EventLog eventLog1;
        private System.IO.FileSystemWatcher fileSystemWatcher1;
        private System.IO.FileSystemWatcher fileSystemWatcher2;
        private System.IO.FileSystemWatcher fileSystemWatcher3;
        private System.IO.FileSystemWatcher fileSystemWatcher4;
        private System.IO.FileSystemWatcher fileSystemWatcher5;
        private System.IO.FileSystemWatcher fileSystemWatcher6;
        private System.IO.FileSystemWatcher fileSystemWatcher7;
    }
}
