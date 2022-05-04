using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using MobiVUE_ATS.DAL;

public partial class WebPages_Reconcilation : System.Web.UI.Page
{
    AssetReconcilation_DAL oDAL;
    public WebPages_Reconcilation()
    {

    }
    ~WebPages_Reconcilation()
    {
        oDAL = null;
    }

    #region PAGE EVENTS
    /// <summary>
    /// Navigates to session expired page in case of user logs off/session expired.
    /// </summary>
    protected void Page_Init(object sender, EventArgs e)
    {
        if (Session["CURRENTUSER"] == null)
        {
            Server.Transfer("SessionExpired.aspx");
        }
        oDAL = new AssetReconcilation_DAL(Session["DATABASE"].ToString());
    }

    /// <summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                btnStart.Enabled = false;
                btnStop.Enabled = false;
                gvCodes.Visible = false;
                gvCodesFacilities.Visible = false;
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                HttpContext.Current.Response.Cache.SetAllowResponseInBrowserHistory(false);
                string _strRights = clsGeneral.GetRights("ASSET_RECONCILIATION", (DataTable)Session["UserRights"]);
                clsGeneral._strRights = _strRights.Split('^');
                clsGeneral.LogUserOperationToLogFile(Session["CURRENTUSER"].ToString(), Session["COMP_NAME"].ToString(), "ASSET_RECONCILIATION");
                if (clsGeneral._strRights[0] == "0")
                {
                    Response.Redirect("UnauthorizedUser.aspx", false);
                }
                GetSiteDetails();
                
            }
        }
        catch (Exception ex)
        {
            HandleExceptions(ex);
        }
    }
    #endregion

    protected void btnStart_Click(object sender, EventArgs e)
    {
        try
        {
            gvCodes.DataSource = null;
            gvCodes.DataBind();
            if(ddlSite.SelectedIndex == 0 || ddlFloor.SelectedIndex == 0)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowErrMsg", "ShowErrMsg('Please select Location and Floor.');", true);
                return;
            }
	        string Site = ddlSite.SelectedValue == "" ? null : ddlSite.SelectedValue;
            string Floor = ddlFloor.SelectedValue == "" ? null : ddlFloor.SelectedValue;
            string Store = ddlStore.SelectedValue == "" ? null : ddlStore.SelectedValue;
            DataTable dc = oDAL.ReconcilationData("", Site, Floor, Store, "", "", Session["CURRENTUSER"].ToString(), "Validate", Session["COMPANY"].ToString());
            if (dc.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowErrMsg", "ShowErrMsg('Please Note : Reconciliation was already initiated.');", true);
                return;
            }
            DataTable dt = oDAL.ReconcilationData("",Site, Floor, Store, "", "", Session["CURRENTUSER"].ToString(), "Generate", Session["COMPANY"].ToString());
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["ReturnId"].ToString() == "0")
                {
                    btnStop.Enabled = true;
                    btnStart.Enabled = false;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowErrMsg", "ShowErrMsg('Please Note : Reconciliation process started successfully.');", true);                    
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowErrMsg", "ShowErrMsg('Please Note : Reconciliation not initiated.');", true);
                    return;
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowErrMsg", "ShowErrMsg('Please Note : Reconciliation not initiated.');", true);
                return;
            }
        }
        catch (Exception ex)
        {
            HandleExceptions(ex);
        }
    }

    #region PRIVATE FUNCTIONS
    /// <summary>
    /// Cathes exception for the entier page operations.
    /// </summary>
    public void HandleExceptions(Exception ex)
    {
        clsGeneral.LogErrorToLogFile(ex, "Reconciliation");
        if (!ex.Message.ToString().Contains("Thread was being aborted."))
        {
            clsGeneral.ErrMsg = ex.Message.ToString(); try { string[] arrErr = ex.Message.ToString().Split('\n'); Session["ErrMsg"] = arrErr[0].ToString().Trim(); }
            catch { }
            Server.Transfer("Error.aspx");
        }
    }
    private void GetSiteDetails()
    {
        DataTable dt = new DataTable();
        ddlSite.DataSource = null;
        dt = oDAL.GetSite(Session["COMPANY"].ToString());
        ddlSite.DataSource = dt;
        ddlSite.DataTextField = "SITE_CODE";
        ddlSite.DataValueField = "SITE_CODE";
        ddlSite.DataBind();
        ddlSite.Items.Insert(0, "-- Select --");
    }
    
    private void GetFloorDropdown(string SiteCode)
    {
        DataTable dt = new DataTable();
        ddlFloor.DataSource = null;
        dt = oDAL.GetFloor(SiteCode, Session["COMPANY"].ToString());
        ddlFloor.DataSource = dt;
        ddlFloor.DataTextField = "FLOOR_CODE";
        ddlFloor.DataValueField = "FLOOR_CODE";
        ddlFloor.DataBind();
        ddlFloor.Items.Insert(0, "-- Select --");
    }
    private void GetStoreDropdown(string Site, string Floor)
    {
        DataTable dt = new DataTable();
        ddlStore.DataSource = null;
        dt = oDAL.GetStore(Site, Floor, Session["COMPANY"].ToString());
        ddlStore.DataSource = dt;
        ddlStore.DataTextField = "STORE_CODE";
        ddlStore.DataValueField = "STORE_CODE";
        ddlStore.DataBind();
        ddlStore.Items.Insert(0, "-- Select --");
    }
    private void LoadGridData()
    {
        string Recid = txtRecId.Text.Trim() == "" ? null : txtRecId.Text.Trim();
        string Site = ddlSite.SelectedValue == "" ? null : ddlSite.SelectedValue;
        string Floor = ddlFloor.SelectedValue == "" ? null : ddlFloor.SelectedValue;
        string Store = ddlStore.SelectedValue == "" ? null : ddlStore.SelectedValue;
        string FromDate = txtFromDate.Text.Trim() == "" ? null : txtFromDate.Text.Trim();
        string ToDate = txtToDate.Text.Trim() == "" ? null : txtToDate.Text.Trim();
        gvCodes.DataSource = null;
        gvCodes.DataBind();
        DataTable dt =  oDAL.ReconcilationData(Recid, Site, Floor, Store, FromDate, ToDate, Session["CURRENTUSER"].ToString(), "GetData", Session["COMPANY"].ToString());
        Session["ReconData"] = dt;
        gvCodes.DataSource = dt;
        gvCodes.DataBind();
        if (Session["COMPANY"].ToString() == "IT")
        {
            gvCodes.Visible = true;
            gvCodes.DataSource = dt;
            gvCodes.DataBind();
        }
        else
        {
            gvCodesFacilities.Visible = true;
            gvCodesFacilities.DataSource = dt;
            gvCodesFacilities.DataBind();
        }
    }
    #endregion

    protected void ddlSite_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            GetFloorDropdown(ddlSite.SelectedValue);
        }
        catch (Exception ex)
        {
            HandleExceptions(ex);
        }        
    }

    protected void ddlFloor_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            btnStart.Enabled = false;
            btnStop.Enabled = false;
            GetStoreDropdown(ddlSite.SelectedValue, ddlFloor.SelectedValue);
            string Site = ddlSite.SelectedValue == "" ? null : ddlSite.SelectedValue;
            string Floor = ddlFloor.SelectedValue == "" ? null : ddlFloor.SelectedValue;
            string Store = ddlStore.SelectedValue == "" ? null : ddlStore.SelectedValue;
            DataTable dt = oDAL.ReconcilationData("", Site, Floor, Store, "", "", Session["CURRENTUSER"].ToString(), "Validate", Session["COMPANY"].ToString());
            if (dt.Rows.Count > 0)
            {
                btnStop.Enabled = true;
            }
            else
            {
                btnStart.Enabled = true;
            }
        }
        catch (Exception ex)
        {
            HandleExceptions(ex);
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            LoadGridData();
        }
        catch (Exception ex)
        {
            HandleExceptions(ex);
        }
    }

    protected void gvCodes_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        {
            gvCodes.PageIndex = e.NewPageIndex;
            LoadGridData();
        }
        catch (Exception ex)
        { HandleExceptions(ex); }
    }

    protected void btnStop_Click(object sender, EventArgs e)
    {
        try
        {
            gvCodes.DataSource = null;
            gvCodes.DataBind();
            if (ddlSite.SelectedIndex == 0 || ddlFloor.SelectedIndex == 0)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowErrMsg", "ShowErrMsg('Please select Location and Floor.');", true);
                return;
            }
            string Site = ddlSite.SelectedValue == "" ? null : ddlSite.SelectedValue;
            string Floor = ddlFloor.SelectedValue == "" ? null : ddlFloor.SelectedValue;
            string Store = ddlStore.SelectedValue == "" ? null : ddlStore.SelectedValue;
            DataTable dc = oDAL.ReconcilationData("", Site, Floor, Store, "", "", Session["CURRENTUSER"].ToString(), "Validate", Session["COMPANY"].ToString());
            if (dc.Rows.Count == 0)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowErrMsg", "ShowErrMsg('Please Note : Reconciliation was already stopped.');", true);
                return;
            }
            DataTable dt = oDAL.ReconcilationData("", Site, Floor, Store, "", "", Session["CURRENTUSER"].ToString(), "Stop", Session["COMPANY"].ToString());
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["ReturnId"].ToString() == "0")
                {
                    btnStart.Enabled = true;
                    btnStop.Enabled = false;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowErrMsg", "ShowErrMsg('Please Note : Reconciliation process stopped successfully.');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowErrMsg", "ShowErrMsg('Please Note : Reconciliation not initiated.');", true);
                    return;
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowErrMsg", "ShowErrMsg('Please Note : Reconciliation not initiated.');", true);
                return;
            }
        }
        catch (Exception ex)
        {
            HandleExceptions(ex);
        }
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        gvCodes.DataSource = null;
        gvCodes.DataBind();
        ddlFloor.SelectedIndex = 0;
        ddlSite.SelectedIndex = 0;
        ddlStore.SelectedIndex = 0;
        btnStop.Enabled = false;
        btnStart.Enabled = false;
    }

    protected void btnExport_Click(object sender, ImageClickEventArgs e)
    {
        try
        {
            if (Session["COMPANY"].ToString() == "IT" && gvCodes.Rows.Count == 0)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowErrMsg", "ShowErrMsg('Please Note : There is no data for being exported.');", true);
                return;
            }
            if (Session["COMPANY"].ToString() != "IT" && gvCodesFacilities.Rows.Count == 0)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowErrMsg", "ShowErrMsg('Please Note : There is no data for being exported.');", true);
                return;
            }
            DataTable dt1 = new DataTable();
            if (Session["ReconData"] != null)
            {
                dt1 = (DataTable)Session["ReconData"];
            }
            if (dt1.Rows.Count > 0)
            {
                dt1.TableName = "ReconcilationData";
                using (ClosedXML.Excel.XLWorkbook wb = new ClosedXML.Excel.XLWorkbook())
                {
                    wb.Worksheets.Add(dt1);
                    wb.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                    wb.Style.Font.Bold = true;
                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=ReconcilationDataReport" + DateTime.Now.ToString("yyyyMMddHHmm") + ".xlsx");

                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowErrMsg", "ShowErrMsg('Please Note : There is no data for being exported.');", true);
                return;
            }
        }
        catch (Exception ex)
        {
            HandleExceptions(ex);
        }
    }

    protected void gvCodesFacilities_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        {
            gvCodesFacilities.PageIndex = e.NewPageIndex;
            LoadGridData();
        }
        catch (Exception ex)
        { HandleExceptions(ex); }
    }

    protected void ddlStore_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            btnStart.Enabled = false;
            btnStop.Enabled = false;
            string Site = ddlSite.SelectedValue == "" ? null : ddlSite.SelectedValue;
            string Floor = ddlFloor.SelectedValue == "" ? null : ddlFloor.SelectedValue;
            string Store = ddlStore.SelectedValue == "" ? null : ddlStore.SelectedValue;
            DataTable dt = oDAL.ReconcilationData("", Site, Floor, Store, "", "", Session["CURRENTUSER"].ToString(), "Validate", Session["COMPANY"].ToString());
            if (dt.Rows.Count > 0)
            {
                btnStop.Enabled = true;
            }
            else
            {
                btnStart.Enabled = true;
            }
        }
        catch (Exception ex)
        { HandleExceptions(ex); }
    }
}