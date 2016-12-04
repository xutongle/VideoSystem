namespace VideoSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deleteManagerLimits : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ManagerLimits", "ManagerID", "dbo.Managers");
            DropIndex("dbo.ManagerLimits", new[] { "ManagerID" });
            DropColumn("dbo.Managers", "ManagerRange");
            DropColumn("dbo.Managers", "ManagerStatus");
            DropTable("dbo.ManagerLimits");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ManagerLimits",
                c => new
                    {
                        LimitID = c.Int(nullable: false, identity: true),
                        LimitAction = c.String(nullable: false),
                        IsAllowed = c.Int(nullable: false),
                        ManagerID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LimitID);
            
            AddColumn("dbo.Managers", "ManagerStatus", c => c.Int(nullable: false));
            AddColumn("dbo.Managers", "ManagerRange", c => c.Int(nullable: false));
            CreateIndex("dbo.ManagerLimits", "ManagerID");
            AddForeignKey("dbo.ManagerLimits", "ManagerID", "dbo.Managers", "ManagerID", cascadeDelete: true);
        }
    }
}
