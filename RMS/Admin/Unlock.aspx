<%@ Page Title="" Language="C#" MasterPageFile="~/RMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="Unlock.aspx.cs" Inherits="RMS.Admin.Unlock" %>
<%@ Register Src="~/Control/RecordPageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../styles/admin.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">    
    <telerik:RadAjaxManager ID="ram" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="dlLocks">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="dlLocks" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap" />

    <uc:PageHeading id="ph1" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">    
    <br />
    <div class="mainContent">
        <asp:Panel ID="pnlNoAccess" runat="server">
            <h4>You do not have the necessary permission to access Administration Tasks for this WSC. Please contact your WSC SIMS Admin if you require access.</h4>
        </asp:Panel>

        <asp:Panel ID="pnlHasAccess" runat="server">
            <asp:Panel ID="pnlNotice" runat="server" CssClass="pnlNotes">
                <h4><asp:Literal ID="ltlNoticeHeading" runat="server" /></h4>
                <asp:Literal ID="ltlNotice" runat="server" />
            </asp:Panel>

            <h3>Currently Locked Records</h3>
            <p>Click the links to unlock the records.</p>
            <asp:DataList ID="dlLocks" runat="server">
                <ItemTemplate>
                    <asp:LinkButton ID="lbClearLock" runat="server" OnCommand="ClearLocks" CommandArgument='<%# Eval("rms_record_id") %>'><%# Eval("Record") %></asp:LinkButton> 
                    - Type: <%# Eval("lock_type") %>, By: <%# Eval("lock_uid") %>, Date: <%# String.Format("{0:MM/dd/yyyy}", Eval("lock_dt")) %><br />
                </ItemTemplate>
            </asp:DataList>
        </asp:Panel>
    </div>
</asp:Content>
