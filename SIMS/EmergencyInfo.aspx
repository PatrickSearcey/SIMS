<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SIMSSafety.Master" CodeBehind="EmergencyInfo.aspx.vb" Inherits="SIMS.EmergencyInfo" %>
<%@ MasterType VirtualPath="~/SIMSSafety.master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<style type="text/css">
    .multiPage
    {
        display: -moz-inline-box;
        display: inline-block;
        zoom: 1;
        *display: inline;
        position: relative;
        margin-bottom: -3px;
        border: 1px solid #416094;
        background-color: #f0f0f4;
    }
    .selectedTab
    {
        font-weight: bold !important;
    }
    .pageViewPanel
    {
        padding: 10px;
    }
    .xlsButton
    {
        color: White;
        border: 0;
        height:48px;
        background: url('../images/excellogo.png') no-repeat center;
        cursor: pointer;
    }
</style>

<telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    <script type="text/javascript">
        function ShowDeleteHospitalForm(id, rowIndex) {
            var grid = $find("<%= rgHospitals.ClientID %>");

            var rowControl = grid.get_masterTableView().get_dataItems()[rowIndex].get_element();
            grid.get_masterTableView().selectItem(rowControl, true);

            window.radopen("Modals/DeleteEmergencyInfo.aspx?hospital_id=" + id, "DeleteDialog");
            return false;
        }
        function ShowDeleteContactForm(id, rowIndex) {
            var grid = $find("<%= rgContacts.ClientID %>");

            var rowControl = grid.get_masterTableView().get_dataItems()[rowIndex].get_element();
            grid.get_masterTableView().selectItem(rowControl, true);

            window.radopen("Modals/DeleteEmergencyInfo.aspx?contact_id=" + id, "DeleteDialog");
            return false;
        }
        function refreshGrid(arg) {
            if (!arg) {
                $find("<%= ram.ClientID %>").ajaxRequest("Rebind");
            }
        }
    </script>
</telerik:RadCodeBlock>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" Runat="Server">
<asp:Panel ID="pnlHasAccess" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server" OnAjaxRequest="ram_AjaxRequest">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="ram">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgHospitals" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="rgContacts" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rgHospitals">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgHospitals" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="ltlNotice1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rgContacts">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgContacts" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="ltlNotice2" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Web20">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadTabStrip runat="server" ID="rts1" Orientation="HorizontalTop" SelectedIndex="0" MultiPageID="rmp1" Skin="Web20">
        <Tabs>
            <telerik:RadTab Text="Hospitals" SelectedCssClass="selectedTab">
            </telerik:RadTab>
            <telerik:RadTab Text="Emergency Contacts" SelectedCssClass="selectedTab">
            </telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
    <telerik:RadMultiPage runat="server" ID="rmp1" SelectedIndex="0" Width="1100px" CssClass="multiPage">
        <telerik:RadPageView runat="server" ID="rpv0">
            <asp:Panel id="pnlHospitals" runat="server" CssClass="pageViewPanel">
                <h4>Manage Hospitals</h4><br />
                <asp:Literal ID="ltlNotice1" runat="server" />
                <telerik:RadGrid ID="rgHospitals" runat="server" 
                    AllowPaging="true" AllowSorting="true" AllowFilteringByColumn="true"
                    OnNeedDataSource="rgHospitals_NeedDataSource" 
                    onPreRender="rgHospitals_PreRender"
                    OnItemCreated="rgHospitals_ItemCreated"
                    onItemCommand="rgHospitals_ItemCommand"
                    OnInsertCommand="rgHospitals_InsertCommand"
                    OnUpdateCommand="rgHospitals_UpdateCommand"
                    Width="98%" Skin="Web20">
                    <SortingSettings SortToolTip="" />
                    <MasterTableView AutoGenerateColumns="False" DataKeyNames="hospital_id" ClientDataKeyNames="hospital_id"
                        Width="100%" CommandItemDisplay="Top" AllowSorting="true" PageSize="20">
                        <PagerStyle Mode="NextPrevNumericAndAdvanced"></PagerStyle>
                        <CommandItemSettings AddNewRecordText="Add New Hospital" ShowRefreshButton="false" />
                        <Columns>
                            <telerik:GridEditCommandColumn ButtonType="ImageButton" />
                            <telerik:GridBoundColumn DataField="hospital_id" HeaderText="Hospital ID" ReadOnly="True" SortExpression="hospital_id" UniqueName="hospital_id" Visible="false"/>
                            <telerik:GridBoundColumn DataField="hospital_nm" HeaderText="Hospital Name" SortExpression="hospital_nm" UniqueName="hospital_nm" HeaderStyle-Width="200" />
                            <telerik:GridBoundColumn DataField="street_addrs" HeaderText="Street Address" SortExpression="street_addrs" UniqueName="street_addrs" HeaderStyle-Width="200" />
                            <telerik:GridBoundColumn DataField="city" HeaderText="City" SortExpression="city" UniqueName="city" HeaderStyle-Width="60" FilterControlWidth="40" />
                            <telerik:GridBoundColumn DataField="state" HeaderText="St" SortExpression="state" UniqueName="state" HeaderStyle-Width="15" AllowFiltering="false" />
                            <telerik:GridBoundColumn DataField="zip" HeaderText="Zip Code" SortExpression="zip" UniqueName="zip" HeaderStyle-Width="50" FilterControlWidth="30" />
                            <telerik:GridBoundColumn DataField="ph_no" HeaderText="Phone No." SortExpression="ph_no" UniqueName="ph_no" FilterControlWidth="60" ItemStyle-Wrap="false" />
                            <telerik:GridBoundColumn DataField="dec_lat_va" HeaderText="Decimal Lat" SortExpression="dec_lat_va" UniqueName="dec_lat_va" AllowFiltering="false" ItemStyle-Wrap="false" />
                            <telerik:GridBoundColumn DataField="dec_long_va" HeaderText="Decimal Long" SortExpression="dec_long_va" UniqueName="dec_long_va" AllowFiltering="false" ItemStyle-Wrap="false" />
                            <telerik:GridTemplateColumn UniqueName="TemplateDeleteColumn" HeaderStyle-Width="30" AllowFiltering="false">
                                <ItemTemplate>
                                    <asp:HyperLink ID="hlDelete" runat="server" Text="Delete"></asp:HyperLink>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>

                        <EditFormSettings EditFormType="Template">
                            <FormTemplate>
                                <div style="padding:5px;background-color:White;">
                                    <table id="tableForm" cellspacing="5" cellpadding="5" width="100%" border="0" rules="none" style="border-collapse: collapse; background: white;">
                                        <tr>
                                            <td colspan="2">
                                                <b>Hospital Details</b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="100px">
                                                <label>Hospital Name:</label>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="tbHospitalNm" Text='<%# Bind("hospital_nm") %>' runat="server" Width="200px" />
                                                <asp:RequiredFieldValidator ID="rfvHospitalNm" runat="server" ControlToValidate="tbHospitalNm" ErrorMessage="*required" Font-Bold="true" Font-Size="X-Small" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <label>Street Address:</label>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="tbStreetAddrs1" Text='<%# Bind("street_addrs") %>' runat="server" Width="200px" />
                                                <asp:RequiredFieldValidator ID="rfvStreetAddrs1" runat="server" ControlToValidate="tbStreetAddrs1" ErrorMessage="*required" Font-Bold="true" Font-Size="X-Small" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <label>City:</label>
                                                            <asp:TextBox ID="tbCity1" Text='<%# Bind("city") %>' runat="server" Width="100px" />
                                                            <asp:RequiredFieldValidator ID="rfvCity1" runat="server" ControlToValidate="tbCity1" ErrorMessage="*" font-bold="true" Font-Size="X-Small" />
                                                        </td>
                                                        <td>
                                                            <label>State:</label>
                                                            <asp:TextBox ID="tbState1" Text='<%# Bind("state") %>' runat="server" Width="25px" />
                                                            <asp:RequiredFieldValidator ID="rfvState1" runat="server" ControlToValidate="tbState1" ErrorMessage="*" Font-Bold="true" Font-Size="X-Small" />
                                                        </td>
                                                        <td>
                                                            <label>Zip:</label>
                                                            <asp:TextBox ID="tbZip1" Text='<%# Bind("zip") %>' runat="server" Width="50px" />
                                                            <asp:RequiredFieldValidator ID="rfvZip1" runat="server" ControlToValidate="tbZip1" ErrorMessage="*" Font-Bold="true" Font-Size="X-Small" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <label>Phone No.:</label>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="tbPhoneNo1" Text='<%# Bind("ph_no") %>' runat="server" Width="80px" />
                                                <asp:RequiredFieldValidator ID="rfvPhoneNo1" runat="server" ControlToValidate="tbPhoneNo1" ErrorMessage="*required" Font-Bold="true" Font-Size="X-Small" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <label>Latitude (dec.):</label>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="tbLat" Text='<%# Bind("dec_lat_va") %>' runat="server" Width="80px" />
                                                <asp:RequiredFieldValidator ID="rfvLat" runat="server" ControlToValidate="tbLat" ErrorMessage="*required" Font-Bold="true" Font-Size="X-Small" />
                                                <span style="font-style:italic;font-size:x-small;"><a href="http://www.mygeoposition.com/" target="_blank">Go here to lookup coordinates!</a></span>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <label>Longitude (dec.):</label>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="tbLong" Text='<%# Bind("dec_long_va") %>' runat="server" Width="80px" />
                                                <asp:RequiredFieldValidator ID="rfvLong" runat="server" ControlToValidate="tbLong" ErrorMessage="*required" Font-Bold="true" Font-Size="X-Small" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right" colspan="2">
                                                <asp:Button ID="btnUpdate1" Text='<%# IIf((TypeOf(Container) is GridEditFormInsertItem), "Insert", "Update") %>' 
                                                    runat="server" CommandName='<%# IIf((TypeOf(Container) is GridEditFormInsertItem), "PerformInsert", "Update") %>' />
                                                <asp:Button ID="btnCancel1" Text="Cancel" runat="server" CausesValidation="false" CommandName="Cancel" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </FormTemplate>
                        </EditFormSettings>
                    </MasterTableView>
                </telerik:RadGrid>
            </asp:Panel>
        </telerik:RadPageView>
        <telerik:RadPageView runat="server" ID="rpv1">
            <asp:Panel id="pnlContacts" runat="server" CssClass="pageViewPanel">
                <h4>Manage Emergency Contacts</h4><br />
                <asp:Literal ID="ltlNotice2" runat="server" />
                <telerik:RadGrid ID="rgContacts" runat="server" AllowPaging="true"
                    OnNeedDataSource="rgContacts_NeedDataSource" 
                    onPreRender="rgContacts_PreRender"
                    OnItemCreated="rgContacts_ItemCreated"                    
                    onItemCommand="rgContacts_ItemCommand"
                    OnInsertCommand="rgContacts_InsertCommand"
                    OnUpdateCommand="rgContacts_UpdateCommand"
                    Width="98%" Skin="Web20" AllowFilteringByColumn="true">
                    <SortingSettings SortToolTip="" />
                    <MasterTableView AutoGenerateColumns="False" DataKeyNames="contact_id" ClientDataKeyNames="contact_id"
                        Width="100%" CommandItemDisplay="Top" AllowSorting="true" PageSize="20">
                        <CommandItemSettings AddNewRecordText="Add New Emergency Contact" ShowRefreshButton="false" />
                        <Columns>
                            <telerik:GridEditCommandColumn ButtonType="ImageButton" HeaderStyle-Width="20" />
                            <telerik:GridBoundColumn DataField="contact_id" HeaderText="Contact ID" ReadOnly="True" SortExpression="contact_id" UniqueName="contact_id" Visible="false"/>
                            <telerik:GridBoundColumn DataField="contact_nm" HeaderText="Contact Name" SortExpression="contact_nm" UniqueName="contact_nm" />
                            <telerik:GridBoundColumn DataField="street_addrs" HeaderText="Street Address" SortExpression="street_addrs" UniqueName="street_addrs" />
                            <telerik:GridBoundColumn DataField="city" HeaderText="City" SortExpression="city" UniqueName="city" HeaderStyle-Width="60" FilterControlWidth="40" />
                            <telerik:GridBoundColumn DataField="state" HeaderText="St" SortExpression="state" UniqueName="state" HeaderStyle-Width="15" AllowFiltering="false" />
                            <telerik:GridBoundColumn DataField="zip" HeaderText="Zip Code" SortExpression="zip" UniqueName="zip" HeaderStyle-Width="50" FilterControlWidth="30" />
                            <telerik:GridBoundColumn DataField="ph_no" HeaderText="Phone No." SortExpression="ph_no" UniqueName="ph_no" ItemStyle-Wrap="false" FilterControlWidth="60" />
                            <telerik:GridTemplateColumn UniqueName="TemplateDeleteColumn" HeaderStyle-Width="30" AllowFiltering="false">
                                <ItemTemplate>
                                    <asp:HyperLink ID="hlDelete" runat="server" Text="Delete"></asp:HyperLink>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                        <EditFormSettings EditFormType="Template">
                            <FormTemplate>
                                <div style="padding:5px;background-color:White;">
                                    <table id="tableForm" cellspacing="5" cellpadding="5" width="100%" border="0" rules="none" style="border-collapse: collapse; background: white;">
                                        <tr>
                                            <td colspan="2">
                                                <b>Emergency Contact Details</b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="100px">
                                                <label>Contact Name:</label>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="tbContactNm" runat="server" Text='<%# Bind("contact_nm") %>'  Width="200px" />
                                                <asp:RequiredFieldValidator ID="rfvContactNm" runat="server" ControlToValidate="tbContactNm" ErrorMessage="*required" Font-Bold="true" Font-Size="X-Small" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <label>Street Address:</label>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="tbStreetAddrs2" runat="server" Text='<%# Bind("street_addrs") %>' Width="200px" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <label>City:</label>
                                                            <asp:TextBox ID="tbCity2" runat="server" Text='<%# Bind("city") %>' Width="100px" />
                                                        </td>
                                                        <td>
                                                            <label>State:</label>
                                                            <asp:TextBox ID="tbState2" runat="server" Text='<%# Bind("state") %>' Width="25px" />
                                                        </td>
                                                        <td>
                                                            <label>Zip:</label>
                                                            <asp:TextBox ID="tbZip2" runat="server" Text='<%# Bind("zip") %>' Width="50px" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <label>Phone No.:</label>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="tbPhoneNo2" runat="server" Text='<%# Bind("ph_no") %>' Width="80px" />
                                                <asp:RequiredFieldValidator ID="rfvPhoneNo2" runat="server" ControlToValidate="tbPhoneNo2" ErrorMessage="*required" Font-Bold="true" Font-Size="X-Small" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right" colspan="2">
                                                <asp:Button ID="btnUpdate2" Text='<%# IIf((TypeOf(Container) is GridEditFormInsertItem), "Insert", "Update") %>' 
                                                    runat="server" CommandName='<%# IIf((TypeOf(Container) is GridEditFormInsertItem), "PerformInsert", "Update") %>' />
                                                <asp:Button ID="btnCancel2" Text="Cancel" runat="server" CausesValidation="false" CommandName="Cancel" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </FormTemplate>
                        </EditFormSettings>
                    </MasterTableView>
                </telerik:RadGrid>
            </asp:Panel>
        </telerik:RadPageView>
    </telerik:RadMultiPage>

    <telerik:RadWindowManager ID="rwm" runat="server">
        <Windows>
            <telerik:RadWindow ID="DeleteDialog" runat="server" Title="Delete emergency info" Height="200px" Skin="Web20"
                width="300px" Left="150px" ReloadOnShow="true" ShowContentDuringLoad="false" Modal="true" />
        </Windows>
    </telerik:RadWindowManager>
</asp:Panel>
</asp:Content>
