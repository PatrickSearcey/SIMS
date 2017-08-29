<%@ Page Title="" Language="C#" MasterPageFile="~/RMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="Records.aspx.cs" Inherits="RMS.Admin.Records" %>
<%@ Register Src="~/Control/RecordPageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../styles/admin.css" rel="stylesheet" />
    <script type="text/javascript">
        function openWin(_id, _type, _SIMS2017URL) {
            if (_type == "newrecord") var oWnd = radopen(_SIMS2017URL + "Modal/RecordEdit.aspx?site_id=" + _id + "&type=" + _type, "rwEditRecords");
            else if (_type == "record") var oWnd = radopen(_SIMS2017URL + "Modal/RecordEdit.aspx?rms_record_id=" + _id + "&type=" + _type, "rwEditRecords");
        }

        function OnClientClose(oWnd, args) {
            //get the transferred arguments
            var arg = args.get_argument();
            if (arg && arg.type == "record") {
                $find("<%= ram.ClientID %>").ajaxRequest("RebindGrids");
            }
    }
    </script>
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
    <telerik:RadWindowManager RenderMode="Lightweight" ID="rwm" ShowContentDuringLoad="false" VisibleStatusbar="false" ReloadOnShow="true" runat="server" EnableShadow="true">
        <Windows>
            <telerik:RadWindow RenderMode="Lightweight" ID="rwEditRecords" runat="server" Behaviors="Close" OnClientClose="OnClientClose" Width="700" Height="600" />
        </Windows>
    </telerik:RadWindowManager>
    <uc:PageHeading id="ph1" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
    <div class="mainContent">
        <br />
        <telerik:RadTabStrip runat="server" ID="rts1" Orientation="HorizontalTop" SelectedIndex="0" MultiPageID="rmp1" Skin="Bootstrap">
            <Tabs>
                <telerik:RadTab Text="Current Record Assignments" SelectedCssClass="selectedTab" SelectedIndex="0" />
                <telerik:RadTab Text="Sites Without A Record Assignment" SelectedCssClass="selectedTab" />
            </Tabs>
        </telerik:RadTabStrip><telerik:RadMultiPage ID="rmp1" runat="server" SelectedIndex="0" Width="100%" CssClass="multiPage">
            <telerik:RadPageView runat="server" ID="rpv0">
                <div class="Filters">
                    <p>
                        Select the desired office and records to pull.
                    </p>
                    <table width="800">
                        <tr>
                            <td><telerik:RadDropDownList ID="rddlOffice" runat="server" DataValueField="office_id" DataTextField="office_nm" Skin="Bootstrap" Width="350px" 
                                DropDownHeight="200px" /></td>
                            <td>Currently Viewing: 
                                <telerik:RadRadioButtonList ID="rrblRecords" runat="server" Direction="Horizontal" AutoPostBack="false">
                                    <Items>
                                        <telerik:ButtonListItem Text="only active records" Value="active" Selected="true" />
                                        <telerik:ButtonListItem Text="all records" Value="all" />
                                    </Items>
                                </telerik:RadRadioButtonList></td>
                            <td><telerik:RadButton ID="rbSubmit" runat="server" OnCommand="UpdateDetails" CommandArgument="Update" Text="Submit" AutoPostBack="true" Skin="Bootstrap" /></td>
                            <td><asp:Literal ID="ltlError" runat="server" /></td>
                        </tr>
                        <tr>
                            <td colspan="4"><span class="filtersSubtext"><asp:Literal ID="ltlNumberOfRecords" runat="server" /></span></td>
                        </tr>
                    </table>
                </div>
                <telerik:RadPanelBar ID="rpbExplanation" runat="server" Width="100%" Skin="Bootstrap">
                    <Items>
                        <telerik:RadPanelItem Text="Explanation (click here to collapse/expand)" Expanded="false">
                            <ContentTemplate>
                                <div style="padding:10px;font-size:9pt;">
                                    <ul>
                                        <li>Clicking on the site number will open the Station Information page</li>
                                        <li>Clicking on the record-type description will open the Record Configuration Interface</li>
                                        <li>The <i>TS Class</i> column shows the classification of the record type; ts means the record has been assigned to a time-series record-type, and nts means the record has been assigned to a non-time-series record-type.</li>
                                        <li>The <i>Cat No</i> column shows the assigned category number for the record. If the category number is 2 or 3, the <i>cat reason</i> column displays the remarks for why the record was categorized this way.</li>
                                        <li>The <i>Parameter</i> column shows the parameter code and name that has been assigned to the record in RMS. If more than one parameter has been assigned to the record, <i>multi-param</i> is displayed.</li>
                                        <li>When viewing <i>all records</i>, rows with light gray font are inactive records.</li>
                                        <li>To assign a new record-type to a site, click on the <i>Assign</i> link</li>
                                        <li><b>Note: Parameter code will not show in this interface until approved daily values are sent to NWISWeb. TS ID assignment can be seen on Records Configuration Interface. Click on Record Type.</b></li>
                                    </ul>
                                </div>
                            </ContentTemplate>
                        </telerik:RadPanelItem>
                    </Items>
                </telerik:RadPanelBar>
                <hr />
                <telerik:RadGrid ID="rgRecords" runat="server" AutoGenerateColumns="false" Skin="Bootstrap" RenderMode="Lightweight"
                    GridLines="None" ShowStatusBar="true" PageSize="50"
                    AllowSorting="true" 
                    AllowMultiRowSelection="false" 
                    AllowFiltering="true"
                    AllowPaging="false"
                    OnNeedDataSource="rgRecords_NeedDataSource" 
                    OnItemDataBound="rgRecords_ItemDataBound"
                    OnPreRender="rgRecords_PreRender">
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
                            <telerik:GridBoundColumn DataField="analyzer_uid" HeaderText="Analyst" UniqueName="analyzer_uid" SortExpression="analyzer_uid" FilterControlWidth="60px" HeaderStyle-Width="80px" />
                            <telerik:GridBoundColumn DataField="approver_uid" HeaderText="Approver" UniqueName="approver_uid" SortExpression="approver_uid" FilterControlWidth="60px" HeaderStyle-Width="80px" />
                            <telerik:GridTemplateColumn DataField="rms_record_id" HeaderText="Record-Type" UniqueName="rms_record_id" AllowFiltering="false" SortExpression="type_ds" HeaderStyle-Width="160px">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lbEditRecord" runat="server"><%# Eval("type_ds") %></asp:LinkButton>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="ts_fg" HeaderText="TS Cat" UniqueName="ts_fg" SortExpression="ts_fg" AllowFiltering="false" HeaderStyle-Width="30px" />
                            <telerik:GridBoundColumn DataField="category_no" HeaderText="Cat No" UniqueName="category_no" SortExpression="category_no" AllowFiltering="false" HeaderStyle-Width="30px" />
                            <telerik:GridBoundColumn DataField="cat_reason" HeaderText="Cat Reason" UniqueName="cat_reason" SortExpression="cat_reason" AllowFiltering="false" />
                            <telerik:GridBoundColumn DataField="ts_full_ds" HeaderText="Parameter" UniqueName="ts_full_ds" SortExpression="ts_full_ds" FilterControlWidth="100px" HeaderStyle-Width="250px" />
                            <telerik:GridTemplateColumn DataField="site_id" UniqueName="site_id" AllowSorting="false" AllowFiltering="false" HeaderStyle-Width="30px">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lbAssignRecord" runat="server" Text="Assign" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </telerik:RadPageView>
            <telerik:RadPageView runat="server" ID="rpv1">
                <telerik:RadGrid ID="rgSites" runat="server" AutoGenerateColumns="false" Skin="Bootstrap" RenderMode="Lightweight"
                    GridLines="None" ShowStatusBar="true" PageSize="50"
                    AllowSorting="true" 
                    AllowMultiRowSelection="false" 
                    AllowFiltering="true"
                    AllowPaging="false"
                    OnNeedDataSource="rgSites_NeedDataSource" 
                    OnItemDataBound="rgSites_ItemDataBound"
                    OnPreRender="rgSites_PreRender">
                    <MasterTableView DataKeyNames="site_id" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="None" 
                        Name="Sites" AllowFilteringByColumn="true">
                        <Columns>
                            <telerik:GridBoundColumn DataField="office_nm" HeaderText="Office" UniqueName="office_nm" SortExpression="office_nm" HeaderStyle-Width="200px" FilterControlWidth="100px" />
                            <telerik:GridTemplateColumn DataField="site_no" HeaderText="Site No." UniqueName="site_no" SortExpression="site_no" HeaderStyle-Width="150px" FilterControlWidth="100px">
                                <ItemTemplate>
                                    <asp:HyperLink ID="hlSiteNo" runat="server" Target="_blank"><%# Eval("site_no") %></asp:HyperLink>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="station_nm" HeaderText="Station Name" UniqueName="station_nm" SortExpression="station_nm" FilterControlWidth="200px"/>
                            <telerik:GridTemplateColumn DataField="site_id" UniqueName="site_id" AllowSorting="false" AllowFiltering="false">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lbAssignRecord" runat="server" Text="Assign" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </telerik:RadPageView>
        </telerik:RadMultiPage>
    </div>
</asp:Content>
