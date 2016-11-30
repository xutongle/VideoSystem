namespace VideoSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createdatabase : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Codes", "UserID", "dbo.Users");
            DropIndex("dbo.Codes", new[] { "UserID" });
            RenameColumn(table: "dbo.Codes", name: "UserID", newName: "User_UserID");
            AddColumn("dbo.Codes", "UserAccount", c => c.Int(nullable: false));
            AddForeignKey("dbo.Codes", "User_UserID", "dbo.Users", "UserID");
            CreateIndex("dbo.Codes", "User_UserID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Codes", new[] { "User_UserID" });
            DropForeignKey("dbo.Codes", "User_UserID", "dbo.Users");
            DropColumn("dbo.Codes", "UserAccount");
            RenameColumn(table: "dbo.Codes", name: "User_UserID", newName: "UserID");
            CreateIndex("dbo.Codes", "UserID");
            AddForeignKey("dbo.Codes", "UserID", "dbo.Users", "UserID", cascadeDelete: true);
        }
    }
}
