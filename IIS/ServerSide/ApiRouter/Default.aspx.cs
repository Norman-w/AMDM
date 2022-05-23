
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AMDM_Server_SDK;
using System.Web.SessionState;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        HttpContextProcessor.Process(Context, new AMDMServerSDK());
    }
}