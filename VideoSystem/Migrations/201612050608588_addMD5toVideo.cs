namespace VideoSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addMD5toVideo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Videos", "VideoMD5", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Videos", "VideoMD5");
        }
    }
}
