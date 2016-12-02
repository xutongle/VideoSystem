namespace VideoSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deleteLog : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Logs", "ManagerID", "dbo.Managers");
            DropIndex("dbo.Logs", new[] { "ManagerID" });
            DropTable("dbo.Logs");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        LogID = c.Int(nullable: false, identity: true),
                        Operate = c.String(nullable: false),
                        OperateTime = c.DateTime(nullable: false),
                        ManagerID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LogID);
            
            CreateIndex("dbo.Logs", "ManagerID");
            AddForeignKey("dbo.Logs", "ManagerID", "dbo.Managers", "ManagerID", cascadeDelete: true);
        }
    }
}
