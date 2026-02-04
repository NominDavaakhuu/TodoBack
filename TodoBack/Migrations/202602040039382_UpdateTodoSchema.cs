namespace TodoBack.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTodoSchema : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TodoItems", "Title", c => c.String());
            DropColumn("dbo.TodoItems", "DeletedAt");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TodoItems", "DeletedAt", c => c.DateTime());
            AlterColumn("dbo.TodoItems", "Title", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
