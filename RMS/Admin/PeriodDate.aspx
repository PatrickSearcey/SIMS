<%@ Page Title="" Language="C#" MasterPageFile="~/RMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="PeriodDate.aspx.cs" Inherits="RMS.Admin.PeriodDate" %>
<%@ Register Src="~/Control/RecordPageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../styles/admin.css" rel="stylesheet" />
    <script type="text/javascript">
        function onClientValueChanging(sender, args) {
            if (!isSliding) return;
            //OADate origin is midnight, 30 December 1899                              

            var date = new Date('December 30,1899 00:00:00');
            date.setDate(date.getDate() + parseInt(args.get_newValue()));

            var tooltip = $find("<%= rttSlider.ClientID %>");
            resetToolTipLocation(tooltip);

            var day = date.getDate();
            var month = date.getMonth() + 1;
            var year = date.getFullYear();

            tooltip.set_text(month + '/' + day + '/' + year); // finnish-localized date format
        }
        function HandleValueChanged(sender, eventArgs) {
            var date = new Date('December 30,1899 00:00:00');
            date.setDate(date.getDate() + parseInt(eventArgs.get_newValue()));

            var day = date.getDate();
            var month = date.getMonth() + 1;
            var year = date.getFullYear();

            var lblEndDate1 = document.getElementById("cph2_lblEndDate1");
            if (lblEndDate1 != null)
                $get("cph2_lblEndDate1").innerText = month + '/' + day + '/' + year;

            $get("cph2_lblBegDate2").innerText = month + '/' + day + '/' + year;
        }
        function HandleValueChanged2(sender, eventArgs) {
            var date = new Date('December 30,1899 00:00:00');
            date.setDate(date.getDate() + parseInt(eventArgs.get_newValue()));

            var day = date.getDate();
            var month = date.getMonth() + 1;
            var year = date.getFullYear();

            $get("cph2_lblEndDate2").innerText = month + '/' + day + '/' + year;
        }

        var isSliding = false;

        function onClientSlideStart(sender, args) {
            isSliding = true;

            var tooltip = $find("<%= rttSlider.ClientID %>");
            showRadToolTip(tooltip, sender);
        }
        function onClientSlide(sender, args) {
            var tooltip = $find("<%= rttSlider.ClientID %>");
            showRadToolTip(tooltip, sender);
        }
        function onClientSlideEnd(sender, args) {
            isSliding = false;

            var tooltip = $find("<%= rttSlider.ClientID %>");
            tooltip.hide();
        }
        function showRadToolTip(tooltip, slider) {
            var activeHandle = slider.get_activeHandle();
            if (!activeHandle) return;

            tooltip.set_targetControl(activeHandle);
            resetToolTipLocation(tooltip);
        }
        function resetToolTipLocation(tooltip) {
            if (!tooltip.isVisible())
                tooltip.show();
            else
                tooltip.updateLocation();
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
                            <telerik:RadButton ID="rbSubmitSite" runat="server" Text="Submit" OnCommand="btnSubmitSite_Command" CommandArgument="Submit" Skin="Bootstrap" />
                            <telerik:RadButton ID="rbCancel" runat="server" Text="Cancel" Skin="Bootstrap" />
                        </div>
                    </fieldset>
                </asp:Panel>
                <asp:Panel ID="pnlEditDates" runat="server">
                    <asp:LinkButton ID="lbReturn" runat="server" Text="&laquo; go back and enter a new site" 
                        OnCommand="lbReturn_Command" CommandName="return" Font-Size="X-Small" /><br /><br />
                    <asp:Label ID="lblSiteNo" runat="server" Font-Bold="true" /><br /><br />
                    <telerik:RadSplitter ID="rsPeriodDates" runat="server" Height="600px" Width="100%" 
                        Orientation="Vertical">
                        <telerik:RadPane ID="rpPeriods" runat="server" Width="390px" BackColor="WhiteSmoke">
                            <div style="padding-right:10px;padding-left:10px;width:90%;">
                                <asp:PlaceHolder ID="phPeriods" runat="server" />
                            </div>
                        </telerik:RadPane>
                        <telerik:RadSplitBar ID="rsb" runat="server" CollapseMode="Forward">
                        </telerik:RadSplitBar>
                        <telerik:RadPane ID="rpEditDates" runat="server">
                            <div style="padding-right:10px;padding-left:10px;width:95%;">
                                <asp:Panel ID="pnlInstructions" runat="server">
                                    <p class="SITitleFontSmall" style="padding-top:10px;">Instructions</p>
                                    <hr />
                                    <ol>
                                        <li>Click on the link for the record and date of choice on the left, and please be patient. The page may take a minute to load.</li>
                                        <li>Modify the date.  If the modification causes another period's date to change, it will be displayed.</li>
                                    </ol>
                                    <p>Limitations of this interface:</p>
                                    <ul>
                                        <li>Assuming more than one record period exists, only the dates of the two most recent periods may be revised.</li>
                                        <li>Dates of locked periods cannot be revised.</li>
                                        <li>If only one period exists for the record, the revisable date range of the begin date is two years prior.</li>
                                    </ul>
                                    <p>For special requests beyond the scope of this interface, please email <a href="mailto:GS-W_Help_SIMS@usgs.gov">GS-W Help SIMS</a>.</p>
                            
                                </asp:Panel>
                                <asp:Panel ID="pnlEdit" runat="server">
                                    <p class="SITitleFontSmall" style="padding-top:10px;">Edit Record Period Dates</p>
                                    <hr />
                                    <asp:Panel id="pnlEditDate" runat="server">
                                        <p>Click and drag the slider to the new record period date. You must click the 
                                        button to Save Changes to finalize the date modification.</p>
                                        <fieldset style="height:250px;width:100%;">
                                            <legend><asp:Literal ID="ltlRecordType" runat="server" /></legend>
                                            <div style="padding:10px;">
                                        
                                                <telerik:RadToolTip  ID="rttSlider" runat="server"  Skin="Bootstrap"
                                                    Position="TopCenter" OffsetY="3" ShowCallout="false" Height="20px" 
                                                    ShowEvent="FromCode" HideEvent="FromCode"></telerik:RadToolTip>
                                            
                                                <asp:Panel ID="pnlEndBeginDates" runat="server" BackColor="PaleGoldenrod">
                                                    <table border="0" cellpadding="2" cellspacing="0">
                                                        <tr>
                                                            <td><asp:Label ID="lblSlider1StartDate" runat="server" /></td>
                                                            <td>
                                                                <telerik:RadSlider runat="server" ID="rsEndBeginDates" Skin="Bootstrap"
                                                                    IsSelectionRangeEnabled="false"
                                                                    ShowDecreaseHandle="false" 
                                                                    ShowIncreaseHandle="false" 
                                                                    OnClientValueChanging="onClientValueChanging" 
                                                                    OnClientSlideStart="onClientSlideStart" 
                                                                    OnClientSlideEnd="onClientSlideEnd" 
                                                                    OnClientSlide="onClientSlide" 
                                                                    OnClientValueChanged="HandleValueChanged" />
                                                            </td>
                                                            <td><asp:Label ID="lblSlider1EndDate" runat="server" /></td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                        
                                                <asp:Panel ID="pnlEndDate" runat="server" BackColor="PaleGoldenrod">
                                                    <table border="0" cellpadding="2" cellspacing="0">
                                                        <tr>
                                                            <td><asp:Label ID="lblSlider2StartDate" runat="server" /></td>
                                                            <td>
                                                                <telerik:RadSlider runat="server" ID="rsEndDate" Skin="Bootstrap"
                                                                    IsSelectionRangeEnabled="false"
                                                                    ShowDecreaseHandle="false" 
                                                                    ShowIncreaseHandle="false"
                                                                    OnClientValueChanging="onClientValueChanging" 
                                                                    OnClientSlideStart="onClientSlideStart" 
                                                                    OnClientSlideEnd="onClientSlideEnd" 
                                                                    OnClientSlide="onClientSlide"
                                                                    OnClientValueChanged="HandleValueChanged2" />
                                                            </td>
                                                            <td><asp:Label ID="lblSlider2EndDate" runat="server" /></td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>  
                                                 <br />
                                                <table cellpadding="5" style="border: 1px solid #b84626;background-color:white;">
                                                    <tr>
                                                        <td style="background-color:#b84626;color:white;text-align:center;">Period ID</td>
                                                        <td style="background-color:#b84626;color:white;text-align:center;">Begin Date</td>
                                                        <td style="background-color:#b84626;color:white;text-align:center;">End Date</td>
                                                        <td style="background-color:#b84626;color:white;text-align:center;">Status</td>
                                                    </tr>
                                                    <tr>
                                                        <td><asp:Literal ID="ltlPeriodID1" runat="server" /></td>
                                                        <td><asp:Literal ID="ltlBegDate1" runat="server" /></td>
                                                        <td><asp:Label ID="lblEndDate1" runat="server" /></td>
                                                        <td><asp:Literal ID="ltlStatus1" runat="server" /></td>
                                                    </tr>
                                                    <tr>
                                                        <td><asp:Literal ID="ltlPeriodID2" runat="server" /></td>
                                                        <td><asp:Label ID="lblBegDate2" runat="server" /></td>
                                                        <td><asp:Label ID="lblEndDate2" runat="server" /></td>
                                                        <td><asp:Literal ID="ltlStatus2" runat="server" /></td>
                                                    </tr>
                                                </table><br />
                                                <telerik:RadButton id="rbEditDates" runat="server" Text="Save Changes" OnCommand="rbEditDates_Command" CommandArgument="EditDates" Skin="Bootstrap" />
                                            </div>
                                        </fieldset>
                                        <asp:HiddenField ID="hfDatesEdited" runat="server" Value="no" />
                                        <asp:HiddenField id="hfDateType" runat="server" />
                                    </asp:Panel>
                                    <asp:Panel ID="pnlConfirm" runat="server" Visible="false">
                                        <p>The date for this record period has been successfully changed!</p>
                                        <p><b>Don't forget to make your analysis match the time periods you just changed!</b></p>
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
