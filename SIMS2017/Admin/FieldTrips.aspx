<%@ Page Title="" Language="C#" MasterPageFile="~/SIMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="FieldTrips.aspx.cs" Inherits="SIMS2017.Admin.FieldTrips" %>
<%@ Register Src="~/Control/SitePageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../styles/admin.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server" OnAjaxRequest="ram_AjaxRequest">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rgFieldTrips">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgFieldTrips" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="lblError" />
                    <telerik:AjaxUpdatedControl ControlID="lblSuccess" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ram">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgFieldTrips" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap" />

    <uc:PageHeading id="ph1" runat="server" />

    <telerik:RadCodeBlock ID="rcb" runat="server">
        <script type="text/javascript">
            function ShowDeleteForm(_id) {
                var oWnd = radopen("../Modal/ConfirmDelete.aspx?type=fieldtrip&ID=" + _id, "rwDeleteDialog");
            }
            function openWin(_id) {
                var oWnd = radopen("../Modal/FieldTrip.aspx?ID=" + _id, "rwFieldTrip");
            }
            function refreshGrid(arg) {
                if (!arg) {
                    $find("<%= ram.ClientID %>").ajaxRequest("Rebind");
                }
            }
            function CheckKey() {
                if (event.keyCode == 13) {
                    CancelEvent();
                }
            }
            function CancelEvent() {
                var e = window.event;

                e.cancelBubble = true;
                e.returnValue = false;
                if (e.stopPropagation) {
                    e.stopPropagation();
                    e.preventDefault();
                }
            }
        </script>
    </telerik:RadCodeBlock>

    <telerik:RadWindowManager ID="rwm" runat="server">
        <Windows>
            <telerik:RadWindow ID="rwDeleteDialog" runat="server" Title="Delete Field Trip" Height="300px" Skin="Bootstrap"
                width="350px" Left="150px" ReloadOnShow="true" ShowContentDuringLoad="false" Modal="true" />
            <telerik:RadWindow ID="rwFieldTrip" runat="server" Title="Field Trip Sites" Height="400px" Skin="Bootstrap"
                width="700px" Left="150px" ReloadOnShow="true" ShowContentDuringLoad="false" Modal="true" />
        </Windows>
    </telerik:RadWindowManager>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
    <div class="mainContent">
        <asp:Panel ID="pnlHasAccess" runat="server">
            <div style="padding-bottom:5px;">
                <asp:Label ID="lblError" runat="server" EnableViewState="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                <asp:Label ID="lblSuccess" runat="server" EnableViewState="False" Font-Bold="True" ForeColor="Green"></asp:Label>
            </div>

            <telerik:RadGrid ID="rgFieldTrips" runat="server" AutoGenerateColumns="false"
                Skin="Bootstrap" GridLines="None" ShowStatusBar="true" PageSize="50"
                AllowSorting="true" 
                AllowMultiRowSelection="false" 
                AllowFiltering="true"
                AllowPaging="false"
                AllowAutomaticDeletes="true" 
                OnNeedDataSource="rgFieldTrips_NeedDataSource"
                OnItemDataBound="rgFieldTrips_ItemDataBound"
                OnInsertCommand="rgFieldTrips_InsertCommand"
                OnUpdateCommand="rgFieldTrips_UpdateCommand"
                OnPreRender="rgFieldTrips_PreRender">
                <PagerStyle Mode="NumericPages" />
                <MasterTableView DataKeyNames="trip_id" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="Top" 
                    Name="Users" AllowFilteringByColumn="true">
                    <CommandItemSettings AddNewRecordText="Add New Field Trip" ShowRefreshButton="false" />
                    <Columns>
                        <telerik:GridEditCommandColumn ButtonType="ImageButton" UniqueName="EditCommandColumn">
                            <HeaderStyle Width="20px" />
                        </telerik:GridEditCommandColumn>
                        <telerik:GridBoundColumn DataField="office_nm" UniqueName="office_nm" HeaderText="Office" FilterControlWidth="150px" />
                        <telerik:GridBoundColumn DataField="trip_nm" UniqueName="trip_nm" HeaderText="Field Trip Name" FilterControlWidth="150px" />
                        <telerik:GridBoundColumn DataField="user_id" UniqueName="user_id" HeaderText="Assigned To" FilterControlWidth="60px" />
                        <telerik:GridTemplateColumn UniqueName="TemplateViewColumn" HeaderStyle-Width="60" AllowFiltering="false" HeaderText="Assigned Sites">
                            <ItemTemplate>
                                <asp:LinkButton ID="lbViewSites" runat="server" Text="View Sites"></asp:LinkButton>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn UniqueName="TemplateDeleteColumn" HeaderStyle-Width="30" AllowFiltering="false" HeaderText="Delete Trip">
                            <ItemTemplate>
                                <asp:LinkButton ID="lbDelete" runat="server" Text="Delete"></asp:LinkButton>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>

                    <EditFormSettings EditFormType="Template">
                        <FormTemplate>
                            <asp:Panel ID="pnlUpdate" runat="server" CssClass="EditForm">
                                <table id="tableForm1" cellspacing="0" cellpadding="2" width="800" border="0" rules="none" style="border-collapse: collapse; background: #ebe2c0;">
                                    <tr>
                                        <td colspan="2">
                                            <h3>Edit Field Trip</h3>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width:180px;">
                                            <label>Trip Name:</label>
                                        </td>
                                        <td>
                                            <telerik:RadTextBox ID="rtbTripName" runat="server" Width="300px" MaxLength="150" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label style="line-height:9pt;">Assigned To:</label>
                                        </td>
                                        <td>
                                            <telerik:RadDropDownList ID="rddlAssignedTo" runat="server" DataTextField="user_nm" DataValueField="user_id" Width="300px" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <h4>Assigned Sites</h4>
                                            <table>
                                                <tr>
                                                    <td valign="top" colspan="2">
                                                        <center><p class="APexptext">Options</p></center>
                                                        <div style="padding-top:5px;">
                                                            <telerik:RadListBox Height="150px" runat="server" ID="rlbSitesStart" EnableDragAndDrop="true" TransferToID="rlbSitesEnd" 
                                                                AllowTransfer="true" Width="450px" Skin="Bootstrap" DataTextField="site_no_nm" DataValueField="site_id" />
                                                        </div>
                                                    </td>
                                                    <td valign="top">
                                                        <center><p class="APexptext">Selected</p></center>
                                                        <div style="padding-top:5px;">
                                                            <telerik:RadListBox Height="150px" runat="server" ID="rlbSitesEnd" EnableDragAndDrop="true" Width="450px" Skin="Bootstrap"
                                                                DataTextField="site_no_nm" DataValueField="site_id" />
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" colspan="2">
                                            <telerik:RadButton ID="rbUpdate" Text="Update" runat="server" CommandName="Update" />
                                            <telerik:RadButton ID="rbCancel1" Text="Cancel" runat="server" CausesValidation="false" CommandName="Cancel" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel ID="pnlInsert" runat="server" CssClass="EditForm">
                                <table id="tableForm2" cellspacing="0" cellpadding="2" width="800" border="0" rules="none" style="border-collapse: collapse; background: #ebe2c0;">
                                    <tr>
                                        <td colspan="2">
                                            <h3>Add New Field Trip</h3>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width:180px;">
                                            <label>Trip Name:</label>
                                        </td>
                                        <td>
                                            <telerik:RadTextBox ID="rtbTripName2" runat="server" Width="300px" MaxLength="250" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label style="line-height:9pt;">Assigned Office:</label>
                                        </td>
                                        <td>
                                            <telerik:RadDropDownList ID="rddlOffice" runat="server" Width="300px" Skin="Bootstrap" DataTextField="office_nm" DataValueField="office_id" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>Assigned To:</label>
                                        </td>
                                        <td>
                                            <telerik:RadDropDownList ID="rddlAssignedTo2" runat="server" DataTextField="user_nm" DataValueField="user_id" Skin="Bootstrap" Width="300px" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <h4>Assigned Sites</h4>
                                            <table>
                                                <tr>
                                                    <td valign="top">
                                                        <center><p class="APexptext">Options</p></center>
                                                        <div style="padding-top:5px;">
                                                            <telerik:RadListBox Height="150px" runat="server" ID="rlbSitesStart2" EnableDragAndDrop="true" TransferToID="rlbSitesEnd2"
                                                                AllowTransfer="true" Width="450px" Skin="Bootstrap" DataTextField="site_no_nm" DataValueField="site_id" />
                                                        </div>
                                                    </td>
                                                    <td valign="top">
                                                        <center><p class="APexptext">Selected</p></center>
                                                        <div style="padding-top:5px;">
                                                            <telerik:RadListBox Height="150px" runat="server" ID="rlbSitesEnd2" EnableDragAndDrop="true" Width="450px" Skin="Bootstrap"
                                                                DataTextField="site_no_nm" DataValueField="site_id" />
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" colspan="2">
                                            <telerik:RadButton ID="rbInsert" Text="Insert" runat="server" CommandName="PerformInsert" />
                                            <telerik:RadButton ID="rbCancel2" Text="Cancel" runat="server" CausesValidation="false" CommandName="Cancel" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </FormTemplate>
                    </EditFormSettings>
                    <EditItemStyle BackColor="#ebe2c0" />
                </MasterTableView>
                <ClientSettings EnableRowHoverStyle="true" EnablePostBackOnRowClick="true" ClientEvents-OnKeyPress="CheckKey" AllowKeyboardNavigation="true">
                    <Selecting AllowRowSelect="True" />
                </ClientSettings>
            </telerik:RadGrid>
        </asp:Panel>
    </div>
</asp:Content>
