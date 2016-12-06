namespace VideoSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deleteVideoMd5 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Videos", "VideoMD5");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Videos", "VideoMD5", c => c.String(nullable: false));
        }
    }
}
