<%@ Page Title="" Language="C#" MasterPageFile="~/SIMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="ThreatenedGages.aspx.cs" Inherits="SIMS2017.Admin.ThreatenedGages" %>
<%@ Register Src="~/Control/SitePageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../styles/admin.css" rel="stylesheet" />
    <script type="text/javascript">
        function openWin(_id, _type, _SIMSURL) {
            oWnd = radopen(_SIMSURL + "Modal/RecordEdit.aspx?rms_record_id=" + _id + "&type=" + _type, "rwEditRecords");
        }

        function OnClientClose(oWnd, args) {
            $find("<%= ram.ClientID %>").ajaxRequest("RebindGrids");
    }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server" OnAjaxRequest="ram_AjaxRequest">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rgThreatenedGages">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgThreatenedGages" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlNotice" />
                    <telerik:AjaxUpdatedControl ControlID="ltlError" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbSubmit">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgThreatenedGages" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="ltlNumberOfRecords" />
                    <telerik:AjaxUpdatedControl ControlID="ltlError" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ram">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgThreatenedGages" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap" />
    <telerik:RadWindowManager RenderMode="Lightweight" ID="rwm" ShowContentDuringLoad="false" VisibleStatusbar="false" ReloadOnShow="true" runat="server" EnableShadow="true">
        <Windows>
            <telerik:RadWindow RenderMode="Lightweight" ID="rwEditRecords" runat="server" Behaviors="Close" OnClientClose="OnClientClose" Width="700" Height="600" />
        </Windows>
    </telerik:RadWindowManager>

    <uc:PageHeading id="ph1" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
    <div class="mainContent">
        <asp:Panel ID="pnlNoAccess" runat="server">
            <h4>You do not have the necessary permission to access Administration Tasks for this WSC. Please contact your WSC SIMS Admin if you require access.</h4>
        </asp:Panel>

        <asp:Panel ID="pnlHasAccess" runat="server">
            <asp:Panel ID="pnlNotice" runat="server" CssClass="pnlNotes">
                <h4><asp:Literal ID="ltlNoticeHeading" runat="server" /></h4>
                <asp:Literal ID="ltlNotice" runat="server" />
            </asp:Panel>

            <div class="Filters">
                <p>Select the desired office.</p>
                <table width="800">
                    <tr>
                        <td width="400px"><telerik:RadDropDownList ID="rddlOffice" runat="server" DataValueField="office_id" DataTextField="office_nm" Skin="Bootstrap" Width="350px" 
                            DropDownHeight="200px" /></td>
                        <td><telerik:RadButton ID="rbSubmit" runat="server" OnCommand="UpdateDetails" CommandArgument="Update" Text="Submit" AutoPostBack="true" Skin="Bootstrap" /></td>
                        <td><asp:Literal ID="ltlError" runat="server" /></td>
                    </tr>
                    <tr>
                        <td colspan="3"><span class="filtersSubtext"><asp:Literal ID="ltlNumberOfRecords" runat="server" /></span></td>
                    </tr>
                </table>
            </div>
            <telerik:RadPanelBar ID="rpbExplanation" runat="server" Width="100%" Skin="Bootstrap">
                <Items>
                    <telerik:RadPanelItem Text="Explanation (click here to collapse/expand)" Expanded="false">
                        <ContentTemplate>
                            <div style="padding:10px;font-size:9pt;">

                            </div>
                        </ContentTemplate>
                    </telerik:RadPanelItem>
                </Items>
            </telerik:RadPanelBar>
            <hr />
            <telerik:RadGrid ID="rgThreatenedGages" runat="server" AutoGenerateColumns="false" Skin="Bootstrap" RenderMode="Lightweight"
                GridLines="None" ShowStatusBar="true" PageSize="50"
                AllowSorting="true" 
                AllowMultiRowSelection="false" 
                AllowFiltering="true"
                AllowPaging="false"
                OnNeedDataSource="rgThreatenedGages_NeedDataSource" 
                OnItemDataBound="rgThreatenedGages_ItemDataBound"
                OnPreRender="rgThreatenedGages_PreRender">
                <GroupingSettings CaseSensitive="false" />
                <MasterTableView DataKeyNames="rms_record_id" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="None" AutoGenerateColumns="false"
                    Name="Records" AllowFilteringByColumn="true">
                    <Columns>
                        <telerik:GridBoundColumn DataField="office_cd" HeaderText="Office" UniqueName="office_cd" SortExpression="office_cd" HeaderStyle-Width="30px" AllowFiltering="false" />
                        <telerik:GridTemplateColumn DataField="site_no" HeaderText="Site No" UniqueName="site_no" SortExpression="site_no" HeaderStyle-Width="150px" FilterControlWidth="100px">
                            <ItemTemplate>
                                <asp:HyperLink ID="hlSiteNo" runat="server" Target="_blank"><%# Eval("site_no") %></asp:HyperLink>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn DataField="station_nm_short" HeaderText="Station Name" UniqueName="station_nm" SortExpression="station_nm" FilterControlWidth="120px"/>
                        <telerik:GridBoundColumn DataField="type_ds" HeaderText="Record-Type" UniqueName="type_ds" AllowFiltering="false" SortExpression="type_ds" HeaderStyle-Width="160px" />
                        <telerik:GridBoundColumn DataField="status" HeaderText="Status" UniqueName="ts_fg" SortExpression="ts_fg" AllowFiltering="false" HeaderStyle-Width="30px" />
                        <telerik:GridBoundColumn DataField="years_of_record" HeaderText="Yrs of Record" UniqueName="category_no" SortExpression="category_no" AllowFiltering="false" HeaderStyle-Width="30px" />
                        <telerik:GridBoundColumn DataField="remarks" HeaderText="Remarks" UniqueName="remarks" SortExpression="remarks" AllowFiltering="false" />
                        <telerik:GridTemplateColumn DataField="rms_record_id" UniqueName="rms_record_id" AllowSorting="false" AllowFiltering="false" HeaderStyle-Width="30px">
                            <ItemTemplate>
                                <asp:LinkButton ID="lbEdit" runat="server" Text="Edit" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </asp:Panel>
    </div>
</asp:Content>
