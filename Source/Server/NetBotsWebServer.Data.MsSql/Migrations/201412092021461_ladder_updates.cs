namespace NetBots.WebServer.Data.MsSql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ladder_updates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GameSummaries", "DateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.GameSummaries", "GameType", c => c.Int(nullable: false));
            DropColumn("dbo.GameSummaries", "TournamentGame");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GameSummaries", "TournamentGame", c => c.Boolean(nullable: false));
            DropColumn("dbo.GameSummaries", "GameType");
            DropColumn("dbo.GameSummaries", "DateTime");
        }
    }
}
