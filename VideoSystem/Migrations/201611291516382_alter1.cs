namespace VideoSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class alter1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Codes", "User_UserID", "dbo.Users");
            DropIndex("dbo.Codes", new[] { "User_UserID" });
            AddColumn("dbo.Codes", "UserID", c => c.Int(nullable: false));
            DropColumn("dbo.Codes", "UserAccount");
            DropColumn("dbo.Codes", "User_UserID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Codes", "User_UserID", c => c.Int());
            AddColumn("dbo.Codes", "UserAccount", c => c.Int(nullable: false));
            DropColumn("dbo.Codes", "UserID");
            CreateIndex("dbo.Codes", "User_UserID");
            AddForeignKey("dbo.Codes", "User_UserID", "dbo.Users", "UserID");
        }
    }
}
