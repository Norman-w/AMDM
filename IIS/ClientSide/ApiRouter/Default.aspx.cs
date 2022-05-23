using System;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        HttpContextProcessor.Process(Context, new AMDMApiProcessor());
        //HttpContextProcessor.Process(Context, null);
        //Context.Response.Write("我 靠!");
    }
}