<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ElemReport.ascx.vb" Inherits="SIMS.ElemReport1" %>

<telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server">
    <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="lbToggleElementEditMode">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="lbToggleElementEditMode" LoadingPanelID="ralp" />
                <telerik:AjaxUpdatedControl ControlID="hfToggleElementEditMode" />
                <telerik:AjaxUpdatedControl ControlID="pnlStaticElemInfo" />
                <telerik:AjaxUpdatedControl ControlID="pnlEditElemInfo" />
            </UpdatedControls>
        </telerik:AjaxSetting>
    </AjaxSettings>
</telerik:RadAjaxManagerProxy>
<asp:Panel ID="pnlContent" runat="server">
    <div style="width:100%;height:40px;">
        <div style="float:left;font-size:8pt;">
            <asp:Literal ID="ltlElemRevisedInfo" runat="server" />
            (<asp:Hyperlink ID="hlRevisionHistory" runat="server" Target="_blank">revision history</asp:Hyperlink>)
        </div>
        <div style="text-align:right;">
            <asp:LinkButton ID="lbToggleElementEditMode" runat="server" OnClick="lbToggleElementEditMode_Click" />
            <asp:HiddenField ID="hfToggleElementEditMode" runat="server" />
        </div>
    </div>
    <h4 class="sectionHeadings"><asp:Literal ID="ltlElemName" runat="server" /></h4>
    <asp:Panel ID="pnlStaticElemInfo" runat="server">
        <p>
            <asp:Literal ID="ltlElemInfo" runat="server" />
        </p>
    </asp:Panel>
    <asp:Panel ID="pnlEditElemInfo" runat="server" Visible="false">
        <br />
        <telerik:RadEditor ID="reElemInfo" runat="server" Skin="Web20" OnClientLoad="OnClientLoad">
            <Tools>
                <telerik:EditorToolGroup>
                    <telerik:EditorTool Name="AjaxSpellCheck" Visible="true" Enabled="true" />
                    <telerik:EditorTool Name="Bold" Visible="true" /> 
                </telerik:EditorToolGroup>
            </Tools>
            <SpellCheckSettings DictionaryLanguage="en-GB" DictionaryPath="~/App_Data/RadSpell/" />
        </telerik:RadEditor>
        <br />
        <asp:Button id="btnSubmitElemInfo" runat="server" Text="save changes and leave edit mode" OnCommand="btnSubmitElemInfo_Command" CommandArgument="editelement" />
        <asp:Button ID="btnCloseElemInfoEditing" runat="server" Text="cancel and leave edit mode without saving changes" OnCommand="btnSubmitElemInfo_Command" CommandArgument="closeediting" />
        <p><asp:Label ID="lblError1" runat="server" EnableViewState="False" Font-Bold="True" ForeColor="Red"></asp:Label>
        <asp:Label ID="lblSuccess1" runat="server" EnableViewState="False" Font-Bold="True" ForeColor="Green"></asp:Label></p>
    </asp:Panel>
</asp:Panel>