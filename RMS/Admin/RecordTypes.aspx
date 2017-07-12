<%@ Page Title="" Language="C#" MasterPageFile="~/RMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="RecordTypes.aspx.cs" Inherits="RMS.Admin.RecordTypes" %>
<%@ Register Src="~/Control/RecordPageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../styles/admin.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server" OnAjaxRequest="ram_AjaxRequest">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rgSites">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgSites" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbSubmit">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgRecords" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="ltlNumberOfRecords" />
                    <telerik:AjaxUpdatedControl ControlID="ltlError" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ram">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgRecords" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="rgSites" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap" />

    <uc:PageHeading id="ph1" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
    <div class="mainContent">
        <telerik:RadGrid ID="rgRecordTypes" runat="server" Width="850px" GridLines="None" AllowPaging="True" PageSize="20" Skin="Sunset"
            AllowSorting="True" AutoGenerateColumns="False" ShowStatusBar="true" OnNeedDataSource="rgRecordTypes_NeedDataSource"
            OnLoad="rgRecordTypes_Load" OnUpdateCommand="rgRecordTypes_UpdateCommand" OnInsertCommand="rgRecordTypes_InsertCommand">
            <MasterTableView Width="100%" EditMode="PopUp" CommandItemSettings-AddNewRecordText="Add new record-type" DataKeyNames="record_type_id">
                <Columns>
                    <telerik:GridEditCommandColumn ButtonType="ImageButton" UniqueName="EditCommandColumn" HeaderText="Edit Record">
                        <HeaderStyle Wrap="true"></HeaderStyle>
                        <ItemStyle CssClass="MyImageButton" Width="5%" />
                    </telerik:GridEditCommandColumn>
                    <telerik:GridBoundColumn UniqueName="type_cd" HeaderText="Type Code" DataField="type_cd" HeaderStyle-Width="200px"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="type_ds" HeaderText="Type Description" DataField="type_ds" HeaderStyle-Width="400px"></telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn HeaderText="Time-Series" SortExpression="ts_fg" UniqueName="ts_fg" HeaderStyle-Width="30px">
                        <ItemTemplate>
                            <asp:Image ID="imgContorNoncont" runat="server" ImageUrl='<%# "../images/" + Eval("ts_fg") + ".png" %>' />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
                <EditFormSettings UserControlName="../Control/RecordTypeConfig.ascx" EditFormType="WebUserControl">
                    <EditColumn UniqueName="EditCommandColumn1">
                    </EditColumn>
                    <PopUpSettings Width="700px" />
                </EditFormSettings>
                <ExpandCollapseColumn ButtonType="ImageButton" Visible="False" UniqueName="ExpandColumn">
                    <HeaderStyle Width="19px"></HeaderStyle>
                </ExpandCollapseColumn>
            </MasterTableView>
        </telerik:RadGrid>
    </div>
</asp:Content>
