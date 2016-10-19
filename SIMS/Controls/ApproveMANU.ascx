<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ApproveMANU.ascx.vb" Inherits="SIMS.ApproveMANU" %>

<style type="text/css">
    .pnlStaticElemInfo {
        background-color: #fff;
        width: 880px;
        margin-left:10px;
    }
    .pnlButtons {
        height: 50px;
        margin-top:10px;
        margin-bottom:0px;
    }
    .btnRefresh {
        border: none;
        vertical-align: middle;
    }
    .footnoteElemAlert {
        background-color: #dbb4b4;
	    margin: 10px 0 10px 0;
	    padding: 10px 10px 10px 10px;
	    color: #565656;	
	    -moz-border-radius: 7px; 
	    -webkit-border-radius: 7px;
	    -khtml-border-radius: 7px;
	    border-radius: 7px; 
	    border: 1px solid rgb(205, 202, 204); 
	    filter: progid:DXImageTransform.Microsoft.Shadow(color=#cdcacc, direction=120, strength=3); 
	    box-shadow: 0px 0px 3px #cdcacc; 
	    -moz-box-shadow: 0 0 3px #cdcacc; 
	    -webkit-box-shadow: 0 0 3px #cdcacc; 
    }
    .remarksElemAlert {
        background-color: #cedbeb;
	    margin: 10px 0 10px 0;
	    padding: 10px 10px 10px 10px;
	    color: #565656;	
	    -moz-border-radius: 7px; 
	    -webkit-border-radius: 7px;
	    -khtml-border-radius: 7px;
	    border-radius: 7px; 
	    border: 1px solid rgb(205, 202, 204); 
	    filter: progid:DXImageTransform.Microsoft.Shadow(color=#cdcacc, direction=120, strength=3); 
	    box-shadow: 0px 0px 3px #cdcacc; 
	    -moz-box-shadow: 0 0 3px #cdcacc; 
	    -webkit-box-shadow: 0 0 3px #cdcacc; 
    }
    .ibIcon {
        float:left;
        padding-right:10px;
        border: none;
    }

</style>

<telerik:RadAjaxManagerProxy ID="ramp" runat="server">
    <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="ibRefresh">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="rlvElem" LoadingPanelID="ralp1" />
            </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="rlvElem">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="rlvElem" />
            </UpdatedControls>
        </telerik:AjaxSetting>
    </AjaxSettings>
</telerik:RadAjaxManagerProxy>
<telerik:RadAjaxLoadingPanel ID="ralp1" runat="server" Skin="Web20" />

<telerik:RadWindow ID="rwFootnotes" runat="server" Width="500px" Height="560px" Modal="false" Skin="Web20" NavigateUrl="~/Modals/FootnotesElems.aspx" />
<telerik:RadWindow ID="rwFootnotesWQ" runat="server" Width="500px" Height="560px" Modal="false" Skin="Web20" NavigateUrl="~/Modals/FootnotesElems.aspx" />
<telerik:RadWindow ID="rwFootnotesCLIM" runat="server" Width="500px" Height="560px" Modal="false" Skin="Web20" NavigateUrl="~/Modals/FootnotesElems.aspx" />
<telerik:RadWindow ID="rwFootnotesECO" runat="server" Width="500px" Height="560px" Modal="false" Skin="Web20" NavigateUrl="~/Modals/FootnotesElems.aspx" />
<telerik:RadWindow ID="rwRemarks" runat="server" Width="360px" Height="400px" Modal="true" Skin="Web20" NavigateUrl="~/Modals/RemarksElems.aspx" />
<telerik:RadWindow ID="rwRemarksWQ" runat="server" Width="360px" Height="400px" Modal="true" Skin="Web20" NavigateUrl="~/Modals/RemarksElems.aspx" />
<telerik:RadWindow ID="rwRemarksCLIM" runat="server" Width="360px" Height="400px" Modal="true" Skin="Web20" NavigateUrl="~/Modals/RemarksElems.aspx" />
<telerik:RadWindow ID="rwRemarksECO" runat="server" Width="360px" Height="400px" Modal="true" Skin="Web20" NavigateUrl="~/Modals/RemarksElems.aspx" />
<telerik:RadWindow ID="rwWC" runat="server" Width="400px" Height="400px" Modal="true" Skin="Web20" NavigateUrl="~/Modals/GWElems.aspx" />
<telerik:RadWindow ID="rwDatum" runat="server" Width="400px" Height="400px" Modal="true" Skin="Web20" NavigateUrl="~/Modals/GWElems.aspx" />

<asp:Panel ID="pnlContent" runat="server" CssClass="roundedPanel">
    <div style="width:100%;height:60px;">
        <div style="float:left;">
            <h3>MANUSCRIPT</h3>
            <asp:Literal ID="ltlSite" runat="server" />
        </div>
        <div style="float:right;font-size:8pt;padding-top:20px;">
            Not seeing your recent manuscript changes? Click the refresh icon!
            <asp:ImageButton ID="ibRefresh" runat="server" CssClass="btnRefresh" OnCommand="ibRefresh_Command" ImageUrl="~/Images/refresh_custom1.png" />
            &nbsp;&nbsp;<asp:ImageButton ID="ibFAQ" runat="server" CssClass="btnRefresh" OnClick="ibFAQ_Click" ImageUrl="~/Images/faqicon.png" />
        </div>
    </div>
    <div style="background-color:#ffffff;">
    <asp:Panel ID="pnlStaticElemInfo" runat="server" CssClass="pnlStaticElemInfo">
        <telerik:RadListView ID="rlvElem" runat="server" ItemPlaceholderID="ElementContainer" 
            OnNeedDataSource="rlvElem_NeedDataSource" OnItemDataBound="rlvElem_ItemDataBound">
            <LayoutTemplate>
                <div style="padding:10px;">
                    <asp:PlaceHolder ID="ElementContainer" runat="server" />
                </div>
            </LayoutTemplate>
            <ItemTemplate>
                <asp:Panel ID="pnlElement" runat="server">
                    <asp:ImageButton ID="ibIcon" runat="server" CssClass="ibIcon" />
                    <p style="font-size:8pt;">Last revised on <asp:Label ID="lblRevisedDt" runat="server" /> by <asp:Label ID="lblRevisedBy" runat="server" />&nbsp;&nbsp;&nbsp;&nbsp;
                        (<asp:HyperLink ID="hlRevisionHistory" runat="server" Text="revision history" Target="_blank" />)
                    </p>
                    <p><b><%#Eval("element_nm")%>.</b>--<%#Eval("element_info")%></p>
                    <telerik:RadToolTip ID="rtt0" runat="server" TargetControlID="ibIcon"
                        RelativeTo="Element" Width="300px" AutoCloseDelay="10000" Skin="Web20"
                        Height="50px" IsClientID="false" Animation="Fade" Position="TopRight">
                        Click to view details about this element in a pop-up window
                    </telerik:RadToolTip>
                </asp:Panel>
            </ItemTemplate>
        </telerik:RadListView>
    </asp:Panel>
    </div>
    <asp:Panel ID="pnlButtons" runat="server" CssClass="pnlButtons">
        <div style="width:100%;">
            <div style="float:left;">
                <asp:Label id="lblComments" runat="server" Text="Approver remarks:" Font-Bold="true" />
                <telerik:RadTextBox ID="rtbComments" runat="server" Height="40px" Width="200px" TextMode="MultiLine" Skin="Web20"  />
                <asp:Button id="btnApprove" runat="server" Text="Approve Manuscript" OnCommand="btnApprove_Command" CommandArgument="approve" CommandName="Cancel" />
                <asp:Literal ID="ltlConfirm" runat="server" />
                <telerik:RadToolTip ID="rtt1" runat="server" TargetControlID="btnApprove"
                    RelativeTo="Element" Width="300px" AutoCloseDelay="10000" Skin="Web20"
                    Height="50px" IsClientID="false" Animation="Fade" Position="TopRight">
                    Are you sure you removed all Personally Identifiable Information?
                </telerik:RadToolTip>
            </div> 
            <div style="float:right;padding-top:10px;">
                <asp:Button ID="btnEdit" runat="server" Text="Edit Manuscript" OnClick="btnEdit_Click" />
                <asp:Button ID="btnCancel" Text="Close Manuscript" runat="server" CausesValidation="False" CommandName="Cancel"></asp:Button>
            </div>
        </div>
    </asp:Panel>
</asp:Panel>