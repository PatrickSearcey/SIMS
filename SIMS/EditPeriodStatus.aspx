<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/RMS.Master" CodeBehind="EditPeriodStatus.aspx.vb" Inherits="SIMS.EditPeriodStatus" %>
<%@ MasterType  virtualPath="~/RMS.master" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxtoolkit" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script type="text/javascript">
    function EnableButton(URL) {
        open(URL);
        document.getElementById('cph1_pnlSetBackStatus').style.display = 'inline';
        document.getElementById('cph1_pnlDialogs').style.display = 'none';
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" Runat="Server">
<asp:Panel ID="pnlHasAccess" runat="server">
    <telerik:RadAjaxPanel ID="rap1" runat="server">
        <asp:Panel ID="pnlEnterSite" runat="server">
            <asp:Label ID="lblNoRecs" runat="server" Text="ALERT: You have entered a site that either has no record periods, or is not registered in RMS.<br /><br />" Font-Bold="true" ForeColor="Red" Visible="false" />
            <fieldset style="width:300px;">
                <legend>Enter Site Number and Agency Code</legend>
                <div style="padding:10px;">
                    <asp:TextBox id="tbSiteNo" runat="server" Width="110" /> &nbsp;&nbsp;
                    <asp:TextBox ID="tbAgencyCd" runat="server" Text="USGS" Width="40" /><br /><br />
                    <asp:Button ID="btnSubmitSite" runat="server" Text="Submit" OnCommand="btnSubmitSite_Command" CommandArgument="Submit" />
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" />
                </div>
            </fieldset>
        </asp:Panel>
        <asp:Panel ID="pnlEditStatus" runat="server">
            <asp:LinkButton ID="lbReturn" runat="server" Text="&laquo; go back and enter a new site" 
                OnCommand="lbReturn_Command" CommandName="return" Font-Size="X-Small" /><br /><br />
            <asp:Label ID="lblSiteNo" runat="server" Font-Bold="true" /><br /><br />
            <telerik:RadSplitter ID="rsPeriodStatus" runat="server" Height="600px" Width="100%" 
                Orientation="Vertical">
                <telerik:RadPane ID="rpPeriods" runat="server" Width="390px" BackColor="WhiteSmoke">
                    <div style="padding-right:10px;padding-left:10px;width:90%;">
                        <asp:PlaceHolder ID="phPeriods" runat="server" />
                    </div>
                </telerik:RadPane>
                <telerik:RadSplitBar ID="rsb" runat="server" CollapseMode="Forward">
                </telerik:RadSplitBar>
                <telerik:RadPane ID="rpEditStatus" runat="server">
                    <div style="padding-right:10px;padding-left:10px;width:95%;">
                        <asp:Panel ID="pnlInstructions" runat="server">
                            <p class="SITitleFontSmall" style="padding-top:10px;">Instructions</p>
                            <hr />
                            <ol>
                                <li>Click on the status link for the record of choice on the left, and please be patient. The page may take a minute to load.</li>
                                <li>Review and save the dialogs that will need to be deleted in order to set the status back.</li>
                                <li>Click the button to set the status back by one level.</li>
                            </ol>
                            <p>Limitations of this interface:</p>
                            <ul>
                                <li>Only the status of the most recent period may be revised.</li>
                                <li>Only periods with a status of Worked, Checked, or Reviewed may be revised.</li>
                                <li>Status of locked periods cannot be revised.</li>
                                <li>The status can only be set back by one level, i.e. Reviewed to Checked, Checked to Worked, or Worked to Working.</li>
                            </ul>
                            <p>For special requests beyond the scope of this interface, please email <a href="mailto:GS-W_Help_SIMS@usgs.gov">GS-W Help SIMS</a>.</p>
                            
                        </asp:Panel>
                        <asp:Panel ID="pnlEdit" runat="server">
                            <p class="SITitleFontSmall" style="padding-top:10px;">Edit Record Period Status</p>
                            <hr />
                            <asp:Panel ID="pnlDialogs" runat="server">
                                <asp:HyperLink ID="hlDialogs" runat="server" ImageUrl="images/dialogsavebutton.png" Font-Bold="true" />
                                <p><b>By setting the status back, the following dialog entries will be deleted:</b></p>
                                <asp:DataList ID="dlDialogs" runat="server">
                                    <HeaderTemplate>
                                        <table cellpadding="5" style="border: 1px solid #863d02;">
                                          <tr>
                                            <td style="background-color:#863d02;color:white;text-align:center;">Dialog Date</td>
                                            <td style="background-color:#863d02;color:white;text-align:center;">Created By</td>
                                            <td style="background-color:#863d02;color:white;text-align:center;">Status Set To</td>
                                            <td style="background-color:#863d02;color:white;text-align:center;">Comments</td>
                                          </tr>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td><%#Container.DataItem("dialog_dt").ToShortDateString%></td>
                                            <td><%#Container.DataItem("dialog_uid")%></td>
                                            <td><%#Container.DataItem("status_set_to_va")%></td>
                                            <td><%#Container.DataItem("comments_va")%></td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </table>
                                    </FooterTemplate>
                                </asp:DataList>
                            </asp:Panel>
                            <asp:Panel id="pnlSetBackStatus" runat="server" CssClass="hidden">
                                <table>
                                    <tr>
                                        <td>
                                            <fieldset style="height:100px;">
                                                <legend>Period Details</legend>
                                                <div style="padding:10px;">
                                                    <label>ID:</label> <asp:Literal ID="ltlPeriodID" runat="server" /><br />
                                                    <label>Begin Date:</label> <asp:Literal ID="ltlBeginDt" runat="server" /><br />
                                                    <label>End Date:</label> <asp:Literal ID="ltlEndDt" runat="server" /><br />
                                                    <label>Status:</label> <asp:Literal ID="ltlStatus" runat="server" />
                                                </div>
                                            </fieldset>
                                        </td>
                                        <td valign="middle">
                                            <img src="images/bigarrow.png" alt="Arrow" />
                                        </td>
                                        <td valign="middle">
                                            <asp:Button id="btnEditStatus" runat="server" OnCommand="btnEditStatus_Command" CommandArgument="EditStatus" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hfStatusEdited" runat="server" Value="no" />
                            </asp:Panel>
                            <asp:Panel ID="pnlConfirm" runat="server" Visible="false">
                                <p>The status for this record period has been successfully changed!</p>
                            </asp:Panel>
                        </asp:Panel>
                    </div>
                </telerik:RadPane>
            </telerik:RadSplitter>
        </asp:Panel>
    </telerik:RadAjaxPanel>
</asp:Panel>
</asp:Content>
