<%@ Page Title="" Language="C#" MasterPageFile="~/RMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="PeriodStatus.aspx.cs" Inherits="RMS.Admin.PeriodStatus" %>
<%@ Register Src="~/Control/RecordPageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../styles/admin.css" rel="stylesheet" />
    <script type="text/javascript">
        function EnableButton(URL) {
            open(URL);
            document.getElementById('cph2_pnlSetBackStatus').style.display = 'inline';
            document.getElementById('cph2_pnlDialogs').style.display = 'none';
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap" />
    <uc:PageHeading id="ph1" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
    <br />
    <div class="mainContent">
        <telerik:RadAjaxPanel ID="rap1" runat="server" LoadingPanelID="ralp">
            <asp:Panel ID="pnlNoAccess" runat="server">
                <h4>You do not have the necessary permission to access Administration Tasks for this WSC. Please contact your WSC SIMS Admin if you require access.</h4>
            </asp:Panel>

            <asp:Panel ID="pnlHasAccess" runat="server">
                <asp:Panel ID="pnlNotice" runat="server" CssClass="pnlNotes">
                    <h4><asp:Literal ID="ltlNoticeHeading" runat="server" /></h4>
                    <asp:Literal ID="ltlNotice" runat="server" />
                </asp:Panel>

                <asp:Panel ID="pnlEnterSite" runat="server">
                    <asp:Label ID="lblNoRecs" runat="server" Text="ALERT: You have entered a site that either has no record periods, or is not registered in RMS.<br /><br />" Font-Bold="true" ForeColor="Red" Visible="false" />
                    <fieldset style="width:300px;">
                        <legend>Enter Site Number and Agency Code</legend>
                        <div style="padding:10px;">
                            <telerik:RadTextBox id="rtbSiteNo" runat="server" Width="150px" Skin="Bootstrap" /> &nbsp;&nbsp;
                            <telerik:RadTextBox ID="rtbAgencyCd" runat="server" Text="USGS" Width="80px" Skin="Bootstrap" /><br /><br />
                            <telerik:RadButton ID="rbSubmitSite" runat="server" Text="Submit" OnCommand="btnSubmitSite_Command" CommandArgument="Submit" />
                            <telerik:RadButton ID="rbCancel" runat="server" Text="Cancel" />
                        </div>
                    </fieldset>
                </asp:Panel>
                <asp:Panel ID="pnlEditStatus" runat="server">
                    <asp:LinkButton ID="lbReturn" runat="server" Text="&laquo; go back and enter a new site" OnCommand="lbReturn_Command" CommandName="return" Font-Size="X-Small" /><br /><br />
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
                                        <li>Only periods with a status of Analyzed or Approved may be revised.</li>
                                        <li>Status of locked periods cannot be revised.</li>
                                        <li>The status can only be set back by one level, i.e. Approved to Analyzed, or Analyzed to Analyzing.</li>
                                    </ul>
                                    <p>For special requests beyond the scope of this interface, please email <a href="mailto:GS-W_Help_SIMS@usgs.gov">GS-W Help SIMS</a>.</p>
                            
                                </asp:Panel>
                                <asp:Panel ID="pnlEdit" runat="server">
                                    <p class="SITitleFontSmall" style="padding-top:10px;">Edit Record Period Status</p>
                                    <hr />
                                    <asp:Panel ID="pnlDialogs" runat="server">
                                        <asp:HyperLink ID="hlDialogs" runat="server" ImageUrl="../images/dialogsavebutton.png" Font-Bold="true" />
                                        <p><b>By setting the status back, the following dialog entries will be deleted:</b></p>
                                        <asp:DataList ID="dlDialogs" runat="server">
                                            <HeaderTemplate>
                                                <table cellpadding="5" style="border: 1px solid #b84626;">
                                                  <tr>
                                                    <td style="background-color:#b84626;color:white;text-align:center;">Dialog Date</td>
                                                    <td style="background-color:#b84626;color:white;text-align:center;">Created By</td>
                                                    <td style="background-color:#b84626;color:white;text-align:center;">Status Set To</td>
                                                    <td style="background-color:#b84626;color:white;text-align:center;">Comments</td>
                                                  </tr>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <tr>
                                                    <td><%# String.Format("{0:MM/dd/yyyy}", Eval("dialog_dt")) %></td>
                                                    <td><%# Eval("dialog_by") %></td>
                                                    <td><%# Eval("status_set_to_va") %></td>
                                                    <td><%# Eval("comments_va") %></td>
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
                                                    <fieldset style="height:120px;">
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
                                                    <img src="../images/bigarrow.png" alt="Arrow" />
                                                </td>
                                                <td valign="middle">
                                                    <telerik:RadButton id="rbEditStatus" runat="server" OnCommand="rbEditStatus_Command" CommandArgument="EditStatus" />
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
            </asp:Panel>
        </telerik:RadAjaxPanel>
    </div>
</asp:Content>
