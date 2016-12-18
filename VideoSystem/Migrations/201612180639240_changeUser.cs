namespace VideoSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Uniq", c => c.String(nullable: false));
            AddColumn("dbo.Users", "From", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "From");
            DropColumn("dbo.Users", "Uniq");
        }
    }
}
