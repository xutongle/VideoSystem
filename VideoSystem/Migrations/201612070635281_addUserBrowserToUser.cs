namespace VideoSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUserBrowserToUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "UserBrowser1", c => c.String(nullable: false));
            AddColumn("dbo.Users", "UserBrowser2", c => c.String(nullable: false));
            AddColumn("dbo.Users", "UserBrowser3", c => c.String(nullable: false));
            DropColumn("dbo.Users", "UserEquipmentType");
            DropColumn("dbo.Users", "EquipmentCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "EquipmentCode", c => c.String(nullable: false));
            AddColumn("dbo.Users", "UserEquipmentType", c => c.String(nullable: false));
            DropColumn("dbo.Users", "UserBrowser3");
            DropColumn("dbo.Users", "UserBrowser2");
            DropColumn("dbo.Users", "UserBrowser1");
        }
    }
}
