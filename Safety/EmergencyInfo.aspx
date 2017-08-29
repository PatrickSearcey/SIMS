<%@ Page Title="" Language="C#" MasterPageFile="~/SafetySingleMenu.Master" AutoEventWireup="true" CodeBehind="EmergencyInfo.aspx.cs" Inherits="Safety.EmergencyInfo" %>
<%@ Register Src="~/Control/SitePageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="styles/reports.css" rel="stylesheet" />
    <script type="text/javascript">
        function ShowDeleteHospitalForm(id, rowIndex) {
            var grid = $find("<%= rgHospitals.ClientID %>");

            var rowControl = grid.get_masterTableView().get_dataItems()[rowIndex].get_element();
            grid.get_masterTableView().selectItem(rowControl, true);

            window.radopen("Modal/DeleteEmergencyInfo.aspx?hospital_id=" + id, "DeleteDialog");
            return false;
        }
        function ShowDeleteContactForm(id, rowIndex) {
            var grid = $find("<%= rgContacts.ClientID %>");

            var rowControl = grid.get_masterTableView().get_dataItems()[rowIndex].get_element();
            grid.get_masterTableView().selectItem(rowControl, true);

            window.radopen("Modal/DeleteEmergencyInfo.aspx?contact_id=" + id, "DeleteDialog");
            return false;
        }
        function refreshGrid(arg) {
            if (!arg) {
                $find("<%= ram.ClientID %>").ajaxRequest("Rebind");
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server"  OnAjaxRequest="ram_AjaxRequest">
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
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap"></telerik:RadAjaxLoadingPanel>

    <uc:PageHeading id="ph1" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
    <br />
    <div class="mainContent">
    <asp:Panel ID="pnlNoAccess" runat="server">
        <h4>You do not have the necessary permission to access Administration Tasks for this WSC. Please contact your WSC SIMS Admin if you require access.</h4>
    </asp:Panel>

    <asp:Panel ID="pnlHasAccess" runat="server">
        <telerik:RadTabStrip runat="server" ID="rts1" Orientation="HorizontalTop" SelectedIndex="0" MultiPageID="rmp1" Skin="Bootstrap">
            <Tabs>
                <telerik:RadTab Text="Hospitals" SelectedCssClass="selectedTab">
                </telerik:RadTab>
                <telerik:RadTab Text="Emergency Contacts" SelectedCssClass="selectedTab">
                </telerik:RadTab>
            </Tabs>
        </telerik:RadTabStrip>
        <telerik:RadMultiPage runat="server" ID="rmp1" SelectedIndex="0" CssClass="multiPage">
            <telerik:RadPageView runat="server" ID="rpv0">
                <asp:Panel id="pnlHospitals" runat="server" CssClass="pageViewPanel">
                    <h4>Manage Hospitals</h4>
                    <asp:Literal ID="ltlNotice1" runat="server" />
                    <telerik:RadGrid ID="rgHospitals" runat="server" 
                        AllowPaging="true" AllowSorting="true" AllowFilteringByColumn="true"
                        OnNeedDataSource="rgHospitals_NeedDataSource" 
                        onPreRender="rgHospitals_PreRender"
                        OnItemCreated="rgHospitals_ItemCreated"
                        onItemCommand="rgHospitals_ItemCommand"
                        OnInsertCommand="rgHospitals_InsertCommand"
                        OnUpdateCommand="rgHospitals_UpdateCommand"
                        Width="98%" Skin="Bootstrap">
                        <SortingSettings SortToolTip="" />
                        <MasterTableView AutoGenerateColumns="False" DataKeyNames="hospital_id" ClientDataKeyNames="hospital_id"
                            Width="100%" CommandItemDisplay="Top" AllowSorting="true" PageSize="20">
                            <PagerStyle Mode="NextPrevNumericAndAdvanced"></PagerStyle>
                            <CommandItemSettings AddNewRecordText="Add New Hospital" ShowRefreshButton="false" />
                            <Columns>
                                <telerik:GridEditCommandColumn ButtonType="ImageButton" />
                                <telerik:GridBoundColumn DataField="hospital_id" HeaderText="Hospital ID" ReadOnly="True" SortExpression="hospital_id" UniqueName="hospital_id" Visible="false"/>
                                <telerik:GridBoundColumn DataField="hospital_nm" HeaderText="Hospital Name" SortExpression="hospital_nm" UniqueName="hospital_nm" HeaderStyle-Width="200px" />
                                <telerik:GridBoundColumn DataField="street_addrs" HeaderText="Street Address" SortExpression="street_addrs" UniqueName="street_addrs" HeaderStyle-Width="200px" />
                                <telerik:GridBoundColumn DataField="city" HeaderText="City" SortExpression="city" UniqueName="city" HeaderStyle-Width="100px" FilterControlWidth="80px" />
                                <telerik:GridBoundColumn DataField="state" HeaderText="St" SortExpression="state" UniqueName="state" HeaderStyle-Width="15px" AllowFiltering="false" />
                                <telerik:GridBoundColumn DataField="zip" HeaderText="Zip Code" SortExpression="zip" UniqueName="zip" HeaderStyle-Width="70px" FilterControlWidth="50px" />
                                <telerik:GridBoundColumn DataField="ph_no" HeaderText="Phone No." SortExpression="ph_no" UniqueName="ph_no" FilterControlWidth="80px" ItemStyle-Wrap="false" />
                                <telerik:GridBoundColumn DataField="dec_lat_va" HeaderText="Decimal Lat" SortExpression="dec_lat_va" UniqueName="dec_lat_va" AllowFiltering="false" ItemStyle-Wrap="false" />
                                <telerik:GridBoundColumn DataField="dec_long_va" HeaderText="Decimal Long" SortExpression="dec_long_va" UniqueName="dec_long_va" AllowFiltering="false" ItemStyle-Wrap="false" />
                                <telerik:GridTemplateColumn UniqueName="TemplateDeleteColumn" HeaderStyle-Width="30px" AllowFiltering="false">
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
                                                <td width="200">
                                                    <label>Hospital Name:</label>
                                                </td>
                                                <td align="left">
                                                    <asp:TextBox ID="tbHospitalNm" Text='<%# Bind("hospital_nm") %>' runat="server" Width="300px" />
                                                    <asp:RequiredFieldValidator ID="rfvHospitalNm" runat="server" ControlToValidate="tbHospitalNm" ErrorMessage="*required" Font-Bold="true" ForeColor="red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td nowrap>
                                                    <label>Street Address:</label>
                                                </td>
                                                <td align="left">
                                                    <asp:TextBox ID="tbStreetAddrs1" Text='<%# Bind("street_addrs") %>' runat="server" Width="300px" />
                                                    <asp:RequiredFieldValidator ID="rfvStreetAddrs1" runat="server" ControlToValidate="tbStreetAddrs1" ErrorMessage="*required" Font-Bold="true" ForeColor="red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <label>City:</label>
                                                                <asp:TextBox ID="tbCity1" Text='<%# Bind("city") %>' runat="server" Width="150px" />
                                                                <asp:RequiredFieldValidator ID="rfvCity1" runat="server" ControlToValidate="tbCity1" ErrorMessage="*" font-bold="true" ForeColor="red" />
                                                            </td>
                                                            <td>
                                                                <label>State:</label>
                                                                <asp:TextBox ID="tbState1" Text='<%# Bind("state") %>' runat="server" Width="50px" />
                                                                <asp:RequiredFieldValidator ID="rfvState1" runat="server" ControlToValidate="tbState1" ErrorMessage="*" Font-Bold="true" ForeColor="red" />
                                                            </td>
                                                            <td>
                                                                <label>Zip:</label>
                                                                <asp:TextBox ID="tbZip1" Text='<%# Bind("zip") %>' runat="server" Width="80px" />
                                                                <asp:RequiredFieldValidator ID="rfvZip1" runat="server" ControlToValidate="tbZip1" ErrorMessage="*" Font-Bold="true" ForeColor="red" />
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
                                                    <asp:TextBox ID="tbPhoneNo1" Text='<%# Bind("ph_no") %>' runat="server" Width="150px" />
                                                    <asp:RequiredFieldValidator ID="rfvPhoneNo1" runat="server" ControlToValidate="tbPhoneNo1" ErrorMessage="*required" Font-Bold="true" ForeColor="red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <label>Latitude (dec.):</label>
                                                </td>
                                                <td align="left">
                                                    <asp:TextBox ID="tbLat" Text='<%# Bind("dec_lat_va") %>' runat="server" Width="120px" />
                                                    <asp:RequiredFieldValidator ID="rfvLat" runat="server" ControlToValidate="tbLat" ErrorMessage="*required" Font-Bold="true" ForeColor="red" />
                                                    <span style="font-style:italic;font-size:x-small;"><a href="http://www.mygeoposition.com/" target="_blank">Go here to lookup coordinates!</a></span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <label>Longitude (dec.):</label>
                                                </td>
                                                <td align="left">
                                                    <asp:TextBox ID="tbLong" Text='<%# Bind("dec_long_va") %>' runat="server" Width="120px" />
                                                    <asp:RequiredFieldValidator ID="rfvLong" runat="server" ControlToValidate="tbLong" ErrorMessage="*required" Font-Bold="true" ForeColor="red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" colspan="2">
                                                    <asp:Button ID="btnUpdate1" Text='<%# ((Container) is GridEditFormInsertItem) ? "Insert" : "Update" %>' 
                                                        runat="server" CommandName='<%# ((Container) is GridEditFormInsertItem) ? "PerformInsert" : "Update" %>' />
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
                    <h4>Manage Emergency Contacts</h4>
                    <asp:Literal ID="ltlNotice2" runat="server" />
                    <telerik:RadGrid ID="rgContacts" runat="server" AllowPaging="true"
                        OnNeedDataSource="rgContacts_NeedDataSource" 
                        onPreRender="rgContacts_PreRender"
                        OnItemCreated="rgContacts_ItemCreated"                    
                        onItemCommand="rgContacts_ItemCommand"
                        OnInsertCommand="rgContacts_InsertCommand"
                        OnUpdateCommand="rgContacts_UpdateCommand"
                        Width="98%" Skin="Bootstrap" AllowFilteringByColumn="true">
                        <SortingSettings SortToolTip="" />
                        <MasterTableView AutoGenerateColumns="False" DataKeyNames="contact_id" ClientDataKeyNames="contact_id"
                            Width="100%" CommandItemDisplay="Top" AllowSorting="true" PageSize="20">
                            <CommandItemSettings AddNewRecordText="Add New Emergency Contact" ShowRefreshButton="false" />
                            <Columns>
                                <telerik:GridEditCommandColumn ButtonType="ImageButton" HeaderStyle-Width="20" />
                                <telerik:GridBoundColumn DataField="contact_id" HeaderText="Contact ID" ReadOnly="True" SortExpression="contact_id" UniqueName="contact_id" Visible="false"/>
                                <telerik:GridBoundColumn DataField="contact_nm" HeaderText="Contact Name" SortExpression="contact_nm" UniqueName="contact_nm" />
                                <telerik:GridBoundColumn DataField="street_addrs" HeaderText="Street Address" SortExpression="street_addrs" UniqueName="street_addrs" />
                                <telerik:GridBoundColumn DataField="city" HeaderText="City" SortExpression="city" UniqueName="city" HeaderStyle-Width="100px" FilterControlWidth="80px" />
                                <telerik:GridBoundColumn DataField="state" HeaderText="St" SortExpression="state" UniqueName="state" HeaderStyle-Width="15px" AllowFiltering="false" />
                                <telerik:GridBoundColumn DataField="zip" HeaderText="Zip Code" SortExpression="zip" UniqueName="zip" HeaderStyle-Width="70px" FilterControlWidth="50px" />
                                <telerik:GridBoundColumn DataField="ph_no" HeaderText="Phone No." SortExpression="ph_no" UniqueName="ph_no" ItemStyle-Wrap="false" FilterControlWidth="70px" />
                                <telerik:GridTemplateColumn UniqueName="TemplateDeleteColumn" HeaderStyle-Width="30px" AllowFiltering="false">
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
                                                <td>
                                                    <label>Contact Name:</label>
                                                </td>
                                                <td align="left">
                                                    <asp:TextBox ID="tbContactNm" runat="server" Text='<%# Bind("contact_nm") %>'  Width="300px" />
                                                    <asp:RequiredFieldValidator ID="rfvContactNm" runat="server" ControlToValidate="tbContactNm" ErrorMessage="*required" Font-Bold="true" ForeColor="red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td nowrap>
                                                    <label>Street Address:</label>
                                                </td>
                                                <td align="left">
                                                    <asp:TextBox ID="tbStreetAddrs2" runat="server" Text='<%# Bind("street_addrs") %>' Width="300px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <label>City:</label>
                                                                <asp:TextBox ID="tbCity2" runat="server" Text='<%# Bind("city") %>' Width="150px" />
                                                            </td>
                                                            <td>
                                                                <label>State:</label>
                                                                <asp:TextBox ID="tbState2" runat="server" Text='<%# Bind("state") %>' Width="50px" />
                                                            </td>
                                                            <td>
                                                                <label>Zip:</label>
                                                                <asp:TextBox ID="tbZip2" runat="server" Text='<%# Bind("zip") %>' Width="80px" />
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
                                                    <asp:TextBox ID="tbPhoneNo2" runat="server" Text='<%# Bind("ph_no") %>' Width="150px" />
                                                    <asp:RequiredFieldValidator ID="rfvPhoneNo2" runat="server" ControlToValidate="tbPhoneNo2" ErrorMessage="*required" Font-Bold="true" ForeColor="red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" colspan="2">
                                                    <asp:Button ID="btnUpdate2" Text='<%# ((Container) is GridEditFormInsertItem) ? "Insert" : "Update" %>' 
                                                        runat="server" CommandName='<%# ((Container) is GridEditFormInsertItem) ? "PerformInsert" : "Update" %>' />
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
                <telerik:RadWindow ID="DeleteDialog" runat="server" Title="Delete emergency info" Height="300px" Skin="Bootstrap"
                    width="500px" Left="150px" ReloadOnShow="true" ShowContentDuringLoad="false" Modal="true" />
            </Windows>
        </telerik:RadWindowManager>
    </asp:Panel>
    </div>
</asp:Content>
