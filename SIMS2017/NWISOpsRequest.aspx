<%@ Page Title="" Language="C#" MasterPageFile="~/SIMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="NWISOpsRequest.aspx.cs" Inherits="SIMS2017.NWISOpsRequest" %>
<%@ Register Src="~/Control/SitePageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="styles/stationinfo.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rrblRequestType">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlSetup" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbNext">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlSetup" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlStepReview" />
                    <telerik:AjaxUpdatedControl ControlID="hfSiteID" />
                    <telerik:AjaxUpdatedControl ControlID="hfRequestType" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="lbReset">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlSetup" />
                    <telerik:AjaxUpdatedControl ControlID="pnlStepReview" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbSend">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlStepReview" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlConfirmSend" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap" />

    <uc:PageHeading id="ph1" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
    <asp:HiddenField ID="hfSiteID" runat="server" />
    <asp:HiddenField ID="hfRequestType" runat="server" />
    <asp:HiddenField ID="hfEmail" runat="server" />
    <div class="mainContent">
        <asp:Panel ID="pnlSetup" runat="server">
            <p style="padding-right:10px;margin-top:20px;">Use this form to ask questions or request support from your local Water Science Center NWIS Operations staff 
            including NWIS DBAs.  Some example requests are: AQ configurations, DECODES, web configuration changes, Annual Data Report configuration, RMS setup, etc.<br /><br />
            This form does not communicate with the national SIMS support staff.  For SIMS support, please E-mail <a href="mailto:GS-W_Help_SIMS@usgs.gov">GS-W Help SIMS</a>.</p>

            <table style="width:600px;" cellpadding="10" cellspacing="0">
                <tr>
                    <td nowrap><b>Name:</b> <asp:Label ID="lblName" runat="server" /></td>
                    <td nowrap><b>Email:</b> <asp:Label ID="lblEmail" runat="server" /></td>
                    <td nowrap><b>Contact Number:</b> <asp:Label ID="lblPhone" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="3" nowrap><b>This request is regarding site number:</b>
                        <asp:Label ID="lblSiteNo" runat="server" />
                        <asp:TextBox ID="tbSiteNo" runat="server" />
                        <asp:Label ID="lblOptional" runat="server" Text="(optional, but very helpful)" Font-Size="X-Small" ForeColor="#800000" />
                    </td>
                </tr>
            </table>
            <br />
            <b>What sort of request is this?</b>
            <telerik:RadRadioButtonList ID="rrblRequestType" runat="server" Direction="Horizontal" Skin="Bootstrap" AutoPostBack="true" OnSelectedIndexChanged="formatSubRequestType">
                <Items>
                    <telerik:ButtonListItem Text="Real-time" Value="Realtime" />
                    <telerik:ButtonListItem Text="General NWIS" Value="GeneralNWIS" />
                    <telerik:ButtonListItem Text="Manuscript Request" Value="MANU" />
                    <telerik:ButtonListItem Text="Other Support" Value="Other" />
                </Items>
            </telerik:RadRadioButtonList>
            <br />
            <asp:Label ID="lblReqTypesHeading" runat="server" Font-Bold="true" Text="Narrow down the subject of your request:" visible="false" />
            <telerik:RadRadioButtonList ID="rrblRTReqTypes" runat="server" Direction="Horizontal" Skin="Bootstrap">
                <Items>
                    <telerik:ButtonListItem Text="DECODES" Value="DECODES" />
                    <telerik:ButtonListItem Text="modify site on web" Value="modifysite" />
                    <telerik:ButtonListItem Text="add/delete site" Value="adddeletesite" />
                    <telerik:ButtonListItem Text="stop/start SATIN" Value="SATIN" />
                    <telerik:ButtonListItem Text="web not updating" Value="web" />
                    <telerik:ButtonListItem Text="general" Value="general" Selected="true" />
                </Items>
            </telerik:RadRadioButtonList>
            <telerik:RadRadioButtonList ID="rrblNWISReqTypes" runat="server" Direction="Horizontal" Skin="Bootstrap">
                <Items>
                    <telerik:ButtonListItem Selected="True" Text="Aquarius" Value="Aquarius" />
                    <telerik:ButtonListItem Text="GWSI" Value="GWSI" />
                    <telerik:ButtonListItem Text="QWDATA" Value="QWDATA" />
                </Items>
            </telerik:RadRadioButtonList>
            <telerik:RadRadioButtonList ID="rrblMANUReqTypes" runat="server" Direction="Horizontal" Skin="Bootstrap">
                <Items>
                    <telerik:ButtonListItem Value="unabletoapprove" Text="MANU not showing up for approval" />
                    <telerik:ButtonListItem Value="MAI" Text="problem using MANU Approval Interface" />
                    <telerik:ButtonListItem Selected="True" Value="general" Text="general" />
                </Items>
            </telerik:RadRadioButtonList>
            <telerik:RadRadioButtonList ID="rrblOtherReqTypes" runat="server" Direction="Horizontal" Skin="Bootstrap">
                <Items>
                    <telerik:ButtonListItem Value="SDESC" Text="station descriptions" />
                    <telerik:ButtonListItem Value="MANU" Text="manuscripts" />
                    <telerik:ButtonListItem Value="unknown" Text="I don't know, please help" Selected="true" />
                </Items>
            </telerik:RadRadioButtonList>

            <p style="padding-right:10px;margin-top:10px;"><b>Enter your request (please be specific):</b></p>
            <asp:TextBox ID="tbRequest" runat="server" Height="100px" Width="600px" Wrap="true" TextMode="MultiLine" />
            <br /><br />
            <telerik:RadButton ID="rbNext" runat="server" Text="Continue to Confirmation" AutoPostBack="true" OnCommand="rbNext_Command" CommandArgument="review" Skin="Bootstrap" />
            <br />
            <asp:Literal ID="ltlError" runat="server" />

        </asp:Panel>

        <asp:Panel ID="pnlStepReview" runat="server" Visible="false">
            <p><b>Review your request:</b></p>
            
            <asp:Literal ID="ltlCompleteRequest" runat="server" /><br />
            
            <b>Your request will be sent to the email address(es) below upon pressing the Send button.<br />
            Use the optional field at the bottom to enter additional email addresses.</b>
            
            <asp:Literal ID="ltlEmails" runat="server" />
            <br /><br />
            <b>Send a carbon copy:</b> <asp:TextBox ID="tbCCEmail" runat="server" Width="300px" />
            <p style="color:#800000;font-size:x-small;padding-left:150px;">&nbsp;&nbsp;&nbsp;<img src="images/underarrow.png" alt="look here!" /> enter email address or USGS userid; use a comma to separate multiple addresses.</p>

            <telerik:RadButton ID="rbSend" runat="server" Text="Send Request" OnCommand="rbSend_Command" CommandName="send" Skin="Bootstrap" /><br /><br />
            <asp:LinkButton ID="lbReset" runat="server" Text="click here to clear fields and start over" OnCommand="lbReset_Command" CommandName="reset" Font-Size="X-Small" />
        </asp:Panel>
        
        <asp:Panel ID="pnlConfirmSend" runat="server" Visible="false">
            <p style="padding-left:10px;padding-right:10px;margin-top:20px;font-weight:bold;">Your request has been successfully sent!  You will receive a response shortly.</p>
        </asp:Panel>
    </div>

</asp:Content>
