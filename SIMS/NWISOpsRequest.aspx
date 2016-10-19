<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SIMSSite.Master" CodeBehind="NWISOpsRequest.aspx.vb" Inherits="SIMS.NWISOpsRequest" %>
<%@ MasterType  virtualPath="~/SIMSSite.master" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxtoolkit" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%--<script type="text/javascript">
    function fileSelected(sender, args) {
        $get("Button1").disabled = "disabled";
    }

    function fileUploaded(sender, args) {
        $get("Button1").disabled = "";
    }
</script>--%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" Runat="Server">
    <p class="SITitleFontSmall">Continue through the prompts to make a help request:</p>
<asp:UpdatePanel ID="upSteps" runat="server">
    <ContentTemplate>
        <asp:Panel ID="pnlStep1" runat="server" BackColor="#f0f0f0" Visible="false">
            <p style="padding-left:10px;padding-right:10px;margin-top:0px;">Use this form to ask questions or 
            request support from your local Water Science Center NWIS Operations staff including NWIS DBAs.  
            Some example requests are: ADAPS configurations, DECODES, web configuration changes, Annual Data 
            Report configuration, RMS setup, etc.<br /><br />
            This form does not communicate with the national SIMS support staff.  For SIMS support, please E-mail 
            <a href="mailto:GS-W_Help_SIMS@usgs.gov">GS-W Help SIMS</a>.</p>
            <table style="width:600px;" cellpadding="10" cellspacing="0">
                <tr>
                    <td nowrap><b>Name:</b> <asp:Label ID="lblName" runat="server" /></td>
                    <td nowrap><b>Email:</b> <asp:Label ID="lblEmail" runat="server" /></td>
                    <td nowrap><b>Contact Number:</b> <asp:Label ID="lblPhone" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="3" nowrap><b>This request is regarding site number:</b>
                        <asp:Label ID="lblSiteNo" runat="server" />
                        <asp:TextBox ID="tbSiteNo" runat="server"  />
                        <asp:Label ID="lblOptional" runat="server" 
                            Text="(optional, but very helpful)" Font-Size="X-Small" ForeColor="#800000" />
                        <asp:Label ID="lblWarning" runat="server" ForeColor="Red" Font-Italic="true" />
                    </td>
                </tr>
            </table>
            <p style="padding-left:10px;padding-right:10px;">
            <asp:Button ID="btnNext1" runat="server" 
                Text="Next &raquo;" OnCommand="Next_Command" CommandName="next1" /></p>
            <p style="padding-left:5px;padding-top:0px;margin-bottom:0px;font-style:italic;color:#800000;height:25px;background-image:url(images/stepstatusbg.gif);background-repeat:repeat-x;">
            Step 1 of 4
            </p>
        </asp:Panel>
        
        <asp:Panel ID="pnlStep2" runat="server" BackColor="#f0f0f0" Visible="false">
            <asp:Label ID="lblTest" runat="server" />
            <p style="padding-left:10px;padding-right:10px;margin-top:0px;"><b>What sort of request is this?</b><br /><br />
            <asp:RadioButtonList ID="rblRequestType" runat="server" RepeatDirection="Horizontal" 
                AutoPostBack="true" OnSelectedIndexChanged="formatSubRequestType">
                <asp:ListItem>Real-time</asp:ListItem>
                <asp:ListItem>General NWIS</asp:ListItem>
                <asp:ListItem>ADR Request</asp:ListItem>
                <asp:ListItem>Other Support</asp:ListItem>
            </asp:RadioButtonList>
            <asp:RequiredFieldValidator ID="rfvRequestType" runat="server" 
                ErrorMessage="You must select a request subject" ControlToValidate="rblRequestType" />
            <br />
            <asp:Label ID="lblReqTypesHeading" runat="server" 
                Font-Bold="true" Text="Narrow down the subject of your request:" visible="false" /><br /><br />
            <asp:RadioButtonList ID="rblRTReqTypes" runat="server" RepeatDirection="Horizontal" Visible="false">
                <asp:ListItem>DECODES</asp:ListItem>
                <asp:ListItem>modify site on web</asp:ListItem>
                <asp:ListItem>add/del site</asp:ListItem>
                <asp:ListItem>stop/start SATIN</asp:ListItem>
                <asp:ListItem>Web not updating</asp:ListItem>
                <asp:ListItem Selected="True">general</asp:ListItem>
            </asp:RadioButtonList>
            <asp:RadioButtonList ID="rblNWISReqTypes" runat="server" RepeatDirection="Horizontal" Visible="false">
                <asp:ListItem Selected="True">ADAPS</asp:ListItem>
                <asp:ListItem>GWSI</asp:ListItem>
                <asp:ListItem>QWDATA</asp:ListItem>
            </asp:RadioButtonList>
            <asp:RadioButtonList ID="rblADRReqTypes" runat="server" RepeatDirection="Horizontal" Visible="false">
                <asp:ListItem>master publication list update</asp:ListItem>
                <asp:ListItem>problem running scripts</asp:ListItem>
                <asp:ListItem Selected="True">general</asp:ListItem>
            </asp:RadioButtonList>
            <asp:RadioButtonList ID="rblOtherReqTypes" runat="server" RepeatDirection="Horizontal" Visible="false">
                <asp:ListItem>station descriptions</asp:ListItem>
                <asp:ListItem>manuscripts</asp:ListItem>
                <asp:ListItem Selected="True">I don't know, please help</asp:ListItem>
            </asp:RadioButtonList>
            <br />
            <asp:Button id="btnBack2" runat="server" Text="&laquo; Back" CausesValidation="false"
                OnCommand="Back_Command" CommandName="back2" />
            <asp:Button ID="btnNext2" runat="server" 
                Text="Next &raquo;" OnCommand="Next_Command" CommandName="next2" /></p>
            <p style="padding-left:5px;padding-top:0px;margin-bottom:0px;font-style:italic;color:#800000;height:25px;background-image:url(images/stepstatusbg.gif);background-repeat:repeat-x;">
            Step 2 of 4
            </p>
        </asp:Panel>
        
        <asp:Panel ID="pnlStep3" runat="server" BackColor="#f0f0f0" Visible="false">
            <p style="padding-left:10px;padding-right:10px;margin-top:0px;">
            <asp:Label ID="lblSubject" runat="server" Font-Bold="true" /><br /><br />
            <b>Enter your request (please be specific):</b><br /><br />
            <asp:TextBox ID="tbRequest" runat="server" Height="100px" Width="600px" Wrap="true" TextMode="MultiLine" />
            <asp:RequiredFieldValidator ID="rfvRequest" runat="server"
                ErrorMessage="You must enter a request" ControlToValidate="tbRequest" />
            <br /><br />
            <asp:Button id="btnBack3" runat="server" Text="&laquo; Back" CausesValidation="false"
                OnCommand="Back_Command" CommandName="back3" />
            <asp:Button ID="btnNext3" runat="server" 
                Text="Next &raquo;" OnCommand="Next_Command" CommandName="next3" /></p>
            <p style="padding-left:5px;padding-top:0px;margin-bottom:0px;font-style:italic;color:#800000;height:25px;background-image:url(images/stepstatusbg.gif);background-repeat:repeat-x;">
            Step 3 of 4
            </p>
        </asp:Panel>
        
        <asp:Panel ID="pnlStep4" runat="server" BackColor="#f0f0f0" Visible="false">
            <p style="padding-left:10px;padding-right:10px;margin-top:0px;"><b>Your request:</b><br />
            <asp:Literal ID="ltlCompleteRequest" runat="server" /><br />
            <b>Your request will be sent to the email address(es) below upon pressing the Send button.<br />
            Use the optional field at the bottom to enter additional email addresses.</b></p>
            <asp:Literal ID="ltlEmails" runat="server" />
            <p style="padding-left:10px;padding-right:10px;">
            <b>Send a carbon copy:</b>
            <asp:TextBox ID="tbCCEmail" runat="server" Width="300px" /><br />
            <span style="color:#800000;font-size:x-small">&nbsp;&nbsp;&nbsp;<img src="images/underarrow.png" alt="look here!" /> enter email address or USGS userid; use a comma to separate multiple addresses.</span>
            <br /><br />
            <p style="padding-left:10px;padding-right:10px;">
            <asp:Button id="btnBack4" runat="server" Text="&laquo; Back"
                OnCommand="Back_Command" CommandName="back4" />
            <asp:Button ID="btnSend" runat="server" 
                Text="Send Request" OnCommand="Next_Command" CommandName="send" /><br /><br />
            <asp:LinkButton ID="lbReset" runat="server" Text="click here to clear fields and start over" 
                OnCommand="lbReset_Command" CommandName="reset" Font-Size="X-Small" /></p>
            <p style="padding-left:5px;padding-top:0px;margin-bottom:0px;font-style:italic;color:#800000;height:25px;background-image:url(images/stepstatusbg.gif);background-repeat:repeat-x;">
            Step 4 of 4
            </p>
        </asp:Panel>
        
        <asp:Panel ID="pnlConfirmSend" runat="server" BackColor="#f0f0f0" Visible="false">
            <p style="padding-left:10px;padding-right:10px;margin-top:0px;margin-bottom:0px;">Your request has been successfully sent!  You will
            receive a response shortly.</p>
        </asp:Panel>
        
        <ajaxToolkit:RoundedCornersExtender ID="rce1" runat="server" 
            TargetControlID="pnlStep1" 
            BorderColor="#97b2dc" 
            Enabled="True" Radius="15" />
        <ajaxToolkit:RoundedCornersExtender ID="rce2" runat="server" 
            TargetControlID="pnlStep2" 
            BorderColor="#97b2dc" 
            Enabled="True" Radius="15" />
        <ajaxToolkit:RoundedCornersExtender ID="rce3" runat="server" 
            TargetControlID="pnlStep3" 
            BorderColor="#97b2dc" 
            Enabled="True" Radius="15" />
        <ajaxToolkit:RoundedCornersExtender ID="rce4" runat="server" 
            TargetControlID="pnlStep4" 
            BorderColor="#97b2dc" 
            Enabled="True" Radius="15" />
        <ajaxToolkit:RoundedCornersExtender ID="rce6" runat="server" 
            TargetControlID="pnlConfirmSend" 
            BorderColor="#97b2dc" 
            Enabled="True" Radius="15" />
         
        <asp:HiddenField ID="hfSiteNo" runat="server" />
        <asp:HiddenField ID="hfOfficeID" runat="server" />
        <asp:HiddenField ID="hfName" runat="server" />
        <asp:HiddenField ID="hfEmail" runat="server" />
        <asp:HiddenField ID="hfPhone" runat="server" />
        <asp:HiddenField ID="hfRequestType" runat="server" />
        <asp:HiddenField ID="hfRequest" runat="server" />
    </ContentTemplate>
</asp:UpdatePanel>
</asp:Content>
