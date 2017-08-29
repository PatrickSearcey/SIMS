<%@ Page Title="" Language="C#" MasterPageFile="~/SIMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="Personnel.aspx.cs" Inherits="SIMS2017.Admin.Personnel"  ValidateRequest="false" %>
<%@ Register Src="~/Control/SitePageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <link href="../styles/admin.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rgPersonnel ">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgPersonnel" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="lblError" />
                    <telerik:AjaxUpdatedControl ControlID="lblSuccess" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="lbToggleActive">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgPersonnel" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="lbToggleActive" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap" />

    <uc:PageHeading id="ph1" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
    <div class="mainContent">
        <asp:Panel ID="pnlNoAccess" runat="server">
            <h4>You do not have the necessary permission to access Administration Tasks for this WSC. Please contact your WSC SIMS Admin if you require access.</h4>
        </asp:Panel>

        <asp:Panel ID="pnlHasAccess" runat="server">
            <telerik:RadScriptBlock ID="rsb" runat="server">
                <script type="text/javascript">
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
            </telerik:RadScriptBlock>

            <div style="padding-bottom:5px;">
                <asp:Label ID="lblError" runat="server" EnableViewState="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                <asp:Label ID="lblSuccess" runat="server" EnableViewState="False" Font-Bold="True" ForeColor="Green"></asp:Label>
                <asp:LinkButton ID="lbToggleActive" runat="server" OnCommand="lbToggleActive_Command" Font-Bold="true" />
            </div>

            <telerik:RadGrid ID="rgPersonnel" runat="server" AutoGenerateColumns="false"
                Skin="Bootstrap" GridLines="None" ShowStatusBar="true" PageSize="50"
                AllowSorting="true" 
                AllowMultiRowSelection="false" 
                AllowFiltering="true"
                AllowPaging="true"
                AllowAutomaticDeletes="true" 
                OnNeedDataSource="rgPersonnel_NeedDataSource"
                OnItemDataBound="rgPersonnel_ItemDataBound"
                OnInsertCommand="rgPersonnel_InsertCommand"
                OnUpdateCommand="rgPersonnel_UpdateCommand"
                OnPreRender="rgPersonnel_PreRender" 
                OnItemCommand="rgPersonnel_ItemCommand">
                <PagerStyle Mode="NumericPages" />
                <MasterTableView DataKeyNames="user_id" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="Top" 
                    Name="Users" AllowFilteringByColumn="true">
                    <CommandItemSettings AddNewRecordText="Add New User" ShowRefreshButton="false" />
                    <Columns>
                        <telerik:GridEditCommandColumn ButtonType="ImageButton" UniqueName="EditCommandColumn">
                            <HeaderStyle Width="20px" />
                        </telerik:GridEditCommandColumn>
                        <telerik:GridBoundColumn DataField="office_nm" UniqueName="office_nm" HeaderText="Office" FilterControlWidth="150px" />
                        <telerik:GridBoundColumn DataField="user_id" UniqueName="user_id" HeaderText="User ID" FilterControlWidth="60px" />
                        <telerik:GridBoundColumn DataField="user_nm" UniqueName="name" HeaderText="Name" FilterControlWidth="100px" />
                        <telerik:GridBoundColumn DataField="administrator_va" UniqueName="administrator_va" HeaderText="Admin Level" FilterControlWidth="50px" />
                        <telerik:GridBoundColumn DataField="pass_access" UniqueName="pass_access" HeaderText="PASS Level" FilterControlWidth="50px" />
                        <telerik:GridBoundColumn DataField="approver_va" UniqueName="approver_va" HeaderText="Safety Approver" />
                        <telerik:GridBoundColumn DataField="active" UniqueName="active" HeaderText="Active" FilterControlWidth="100px" />
                        <telerik:GridBoundColumn DataField="show_reports" UniqueName="show_reports" Display="false" />
                    </Columns>

                    <EditFormSettings EditFormType="Template">
                        <FormTemplate>
                            <asp:Panel ID="pnlUpdate" runat="server" CssClass="EditForm">
                                <table id="tableForm1" cellspacing="0" cellpadding="2" width="800" border="0" rules="none" style="border-collapse: collapse; background: #ebe2c0;">
                                    <tr>
                                        <td colspan="2">
                                            <h4>Edit User Information</h4>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width:180px;">
                                            <label>User ID:</label>
                                        </td>
                                        <td>
                                            <telerik:RadTextBox ID="rtbUserID" runat="server" Width="100px" MaxLength="20" Enabled="false" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label style="line-height:9pt;">Name to appear on<br />Analysis Notes:</label>
                                        </td>
                                        <td>
                                            <label>First Name:</label> <telerik:RadTextBox ID="rtbFirstNm" runat="server" Width="130px" MaxLength="50" />
                                            <label>Last Name:</label> <telerik:RadTextBox ID="rtbLastNm" runat="server" Width="130px" MaxLength="50" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>Administrator:</label>
                                        </td>
                                        <td>
                                            <telerik:RadDropDownList ID="rddlAdminLevel" runat="server" Width="100px" DataTextField="level" DataValueField="level" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>PASS Maintainer Level:</label>
                                        </td>
                                        <td>
                                            <telerik:RadDropDownList ID="rddlPASSLevel" runat="server" Width="100px" DataTextField="level" DataValueField="level" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>Safety Approver:</label>
                                        </td>
                                        <td>
                                            <telerik:RadRadioButtonList ID="rrblSafetyApprover" runat="server">
                                                <Items>
                                                    <telerik:ButtonListItem Text="Yes" Value="true" />
                                                    <telerik:ButtonListItem Text="No" Value="false" />
                                                </Items>
                                            </telerik:RadRadioButtonList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>Assigned Office:</label>
                                        </td>
                                        <td>
                                            <telerik:RadDropDownList ID="rddlOffice" runat="server" Width="300px" DataTextField="office_nm" DataValueField="office_id" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>Status:</label>
                                        </td>
                                        <td>
                                            <telerik:RadRadioButtonList ID="rrblStatus" runat="server">
                                                <Items>
                                                    <telerik:ButtonListItem Text="Active" Value="true" />
                                                    <telerik:ButtonListItem Text="Inactive" Value="false" />
                                                </Items>
                                            </telerik:RadRadioButtonList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>Show in Progress Reports:</label>
                                        </td>
                                        <td>
                                            <telerik:RadCheckBox ID="rcbReports" runat="server" />
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
                            <asp:Panel ID="pnlInsert1" runat="server" CssClass="EditForm">
                                <h4>Lookup a user in Active Directory by their user ID<br />
                                    <i>NOTE: This is not necessarily their E-mail alias, but the user ID they use to logon to their computer.</i>
                                </h4>
                                <div style="padding:10px;">
                                    <label>Enter the user ID:</label> <telerik:RadTextBox ID="rtbUserID2" runat="server" Width="130px" MaxLength="20" />
                                    <telerik:RadButton ID="rbValidateUser" Text="Find User" runat="server" CommandName="ValidateUser" />
                                </div>
                            </asp:Panel>
                            <asp:Panel ID="pnlInsert2" runat="server" CssClass="EditForm">
                                <table id="tableForm2" cellspacing="0" cellpadding="2" width="800" border="0" rules="none" style="border-collapse: collapse; background: #ebe2c0;">
                                    <tr>
                                        <td colspan="2">
                                            <h4>Add New User</h4>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width:180px;">
                                            <label>User ID:</label>
                                        </td>
                                        <td>
                                            <telerik:RadTextBox ID="rtbUserID3" runat="server" Width="100px" MaxLength="20" Enabled="false" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label style="line-height:9pt;">Name to appear on<br />Analysis Notes:</label>
                                        </td>
                                        <td>
                                            <label>First Name:</label> <telerik:RadTextBox ID="rtbFirstNm2" runat="server" Width="100px" MaxLength="50" />
                                            <label>Last Name:</label> <telerik:RadTextBox ID="rtbLastNm2" runat="server" Width="100px" MaxLength="50" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>Administrator:</label>
                                        </td>
                                        <td>
                                            <telerik:RadDropDownList ID="rddlAdminLevel2" runat="server" Width="100px" DataTextField="level" DataValueField="level" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>PASS Maintainer Level:</label>
                                        </td>
                                        <td>
                                            <telerik:RadDropDownList ID="rddlPASSLevel2" runat="server" Width="100px" DataTextField="level" DataValueField="level" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>Safety Approver:</label>
                                        </td>
                                        <td>
                                            <telerik:RadRadioButtonList ID="rrblSafetyApprover2" runat="server">
                                                <Items>
                                                    <telerik:ButtonListItem Text="Yes" Value="true" />
                                                    <telerik:ButtonListItem Text="No" Value="false" />
                                                </Items>
                                            </telerik:RadRadioButtonList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>Assigned Office:</label>
                                        </td>
                                        <td>
                                            <telerik:RadDropDownList ID="rddlOffice2" runat="server" Width="300px" DataTextField="office_nm" DataValueField="office_id" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>Status:</label>
                                        </td>
                                        <td>
                                            <telerik:RadRadioButtonList ID="rrblStatus2" runat="server">
                                                <Items>
                                                    <telerik:ButtonListItem Text="Active" Value="true" />
                                                    <telerik:ButtonListItem Text="Inactive" Value="false" />
                                                </Items>
                                            </telerik:RadRadioButtonList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>Show in Progress Reports:</label>
                                        </td>
                                        <td>
                                            <telerik:RadCheckBox ID="rcbReports2" runat="server" />
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
