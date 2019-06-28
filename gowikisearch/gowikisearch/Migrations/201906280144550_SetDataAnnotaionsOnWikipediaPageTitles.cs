namespace gowikisearch.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetDataAnnotaionsOnWikipediaPageTitles : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.WikipediaPageTitles", "Title", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.WikipediaPageTitles", "Title", c => c.String());
        }
    }
}
