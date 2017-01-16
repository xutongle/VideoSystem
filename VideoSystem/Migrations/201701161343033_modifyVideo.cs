namespace VideoSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modifyVideo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Videos",
                c => new
                    {
                        VideoID = c.Int(nullable: false, identity: true),
                        ls_video_id = c.Int(nullable: false),
                        ls_video_uuid = c.String(nullable: false),
                        VideoName = c.String(nullable: false),
                        UploadTime = c.DateTime(nullable: false),
                        CodeCounts = c.Int(nullable: false),
                        CodeUsed = c.Int(nullable: false),
                        CodeNotUsed = c.Int(nullable: false),
                        VideoImageLocal = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.VideoID);
            
            CreateTable(
                "dbo.Codes",
                c => new
                    {
                        CodeID = c.Int(nullable: false, identity: true),
                        CodeValue = c.String(nullable: false),
                        CodeStatus = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                        VideoID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CodeID)
                .ForeignKey("dbo.Videos", t => t.VideoID, cascadeDelete: true)
                .Index(t => t.VideoID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        UserAccount = c.String(nullable: false),
                        UserPassword = c.String(nullable: false),
                        UserEmail = c.String(nullable: false),
                        UserPhone = c.String(nullable: false),
                        UserBrowser1 = c.String(nullable: false),
                        UserBrowser2 = c.String(nullable: false),
                        UserBrowser3 = c.String(nullable: false),
                        Uniq = c.String(nullable: false),
                        From = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.UserID);
            
            CreateTable(
                "dbo.Suggests",
                c => new
                    {
                        SuggestID = c.Int(nullable: false, identity: true),
                        CreateTime = c.DateTime(nullable: false),
                        SuggestText = c.String(nullable: false),
                        UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SuggestID)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.Managers",
                c => new
                    {
                        ManagerID = c.Int(nullable: false, identity: true),
                        ManagerAccount = c.String(nullable: false),
                        ManagerPassword = c.String(nullable: false),
                        ManagerEmail = c.String(nullable: false),
                        ManagerPhone = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ManagerID);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductID = c.Int(nullable: false, identity: true),
                        ProductPrice = c.Int(nullable: false),
                        ProductName = c.String(nullable: false),
                        ProductImg = c.String(nullable: false),
                        ProductUrl = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ProductID);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Suggests", new[] { "UserID" });
            DropIndex("dbo.Codes", new[] { "VideoID" });
            DropForeignKey("dbo.Suggests", "UserID", "dbo.Users");
            DropForeignKey("dbo.Codes", "VideoID", "dbo.Videos");
            DropTable("dbo.Products");
            DropTable("dbo.Managers");
            DropTable("dbo.Suggests");
            DropTable("dbo.Users");
            DropTable("dbo.Codes");
            DropTable("dbo.Videos");
        }
    }
}
