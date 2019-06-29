namespace gowikisearch.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate1 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.WikipediaPageTitles", newName: "WikipediaPageTitle");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.WikipediaPageTitle", newName: "WikipediaPageTitles");
        }
    }
}
