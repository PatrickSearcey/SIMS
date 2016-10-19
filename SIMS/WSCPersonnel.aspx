<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SIMSSite.Master" CodeBehind="WSCPersonnel.aspx.vb" Inherits="SIMS.WSCPersonnel" %>
<%@ MasterType  virtualPath="~/SIMSSite.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .EditForm {
            padding:5px;
            background-color:#cfe3db;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Web20">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxPanel ID="rap" runat="server" LoadingPanelID="ralp">
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
        <div style="border:none;height:40px;">
            <div style="float:left;">
                <asp:Label ID="lblError" runat="server" EnableViewState="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                <asp:Label ID="lblSuccess" runat="server" EnableViewState="False" Font-Bold="True" ForeColor="Green"></asp:Label><br />
                <asp:LinkButton ID="lbToggleActive" runat="server" OnCommand="lbToggleActive_Command" Font-Bold="true" />
            </div>
        </div>
        <telerik:RadGrid ID="rgPersonnel" runat="server" AutoGenerateColumns="false" EnableLinqExpressions="false" 
            Skin="Web20" GridLines="None" ShowStatusBar="true" PageSize="50"
            AllowSorting="true" 
            AllowMultiRowSelection="false" 
            AllowFiltering="true"
            AllowPaging="true"
            AllowAutomaticDeletes="true" 
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
                        <ItemStyle CssClass="MyImageButton" />
                    </telerik:GridEditCommandColumn>
                    <telerik:GridBoundColumn DataField="office_nm" UniqueName="office_nm" HeaderText="Office" FilterControlWidth="100px" />
                    <telerik:GridBoundColumn DataField="user_id" UniqueName="user_id" HeaderText="User ID" FilterControlWidth="60px" />
                    <telerik:GridBoundColumn DataField="user_nm" UniqueName="name" HeaderText="Name" FilterControlWidth="100px" />
                    <telerik:GridBoundColumn DataField="first_nm" UniqueName="first_nm" Display="false" />
                    <telerik:GridBoundColumn DataField="last_nm" UniqueName="last_nm" Display="false" />
                    <telerik:GridBoundColumn DataField="administrator_va" UniqueName="administrator_va" HeaderText="Admin Level" FilterControlWidth="50px" />
                    <telerik:GridBoundColumn DataField="pass_access" UniqueName="pass_access" HeaderText="PASS Level" FilterControlWidth="50px" />
                    <telerik:GridBoundColumn DataField="approver_va" UniqueName="approver_va" HeaderText="Safety Approver" />
                    <telerik:GridBoundColumn DataField="active" UniqueName="active" HeaderText="Active" AllowFiltering="false" />
                    <telerik:GridBoundColumn DataField="show_reports" UniqueName="show_reports" Display="false" />
                </Columns>

                <EditFormSettings EditFormType="Template">
                    <FormTemplate>
                        <asp:Panel ID="pnlUpdate" runat="server" CssClass="EditForm">
                            <table id="tableForm1" cellspacing="5" cellpadding="5" width="600" border="0" rules="none" style="border-collapse: collapse; background: #cfe3db;">
                                <tr>
                                    <td colspan="2">
                                        <h4>Edit User Information</h4>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width:120px;">
                                        <label>User ID:</label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tbUserID" runat="server" Width="100px" MaxLength="20" Enabled="false" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>Name to appear on Analysis Notes:</label>
                                    </td>
                                    <td>
                                        <label>First Name:</label> <asp:TextBox ID="tbFirstNm" runat="server" Width="100px" MaxLength="50" />
                                        <label>Last Name:</label> <asp:TextBox ID="tbLastNm" runat="server" Width="100px" MaxLength="50" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>Administrator:</label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlAdminLevel" runat="server" Width="100px" DataTextField="level" DataValueField="level" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>PASS Maintainer Level:</label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlPASSLevel" runat="server" Width="100px" DataTextField="level" DataValueField="level" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>Safety Approver:</label>
                                    </td>
                                    <td>
                                        <asp:RadioButtonList ID="rblSafetyApprover" runat="server" RepeatDirection="Horizontal">
                                            <asp:ListItem Text="Yes" Value="yes"></asp:ListItem>
                                            <asp:ListItem Text="No" Value="no"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>Assigned Office:</label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlOffice" runat="server" Width="300px" DataTextField="office_nm" DataValueField="office_cd" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>Status:</label>
                                    </td>
                                    <td>
                                        <asp:RadioButtonList ID="rblStatus" runat="server" RepeatDirection="Horizontal">
                                            <asp:ListItem Text="Active" Value="true"></asp:ListItem>
                                            <asp:ListItem Text="Inactive" Value="false"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>Show in Progress Reports:</label>
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="cbReports" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" colspan="2">
                                        <asp:Button ID="btnUpdate" Text="Update" runat="server" CommandName="Update" />
                                        <asp:Button ID="btnCancel1" Text="Cancel" runat="server" CausesValidation="false" CommandName="Cancel" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <asp:Panel ID="pnlInsert1" runat="server" CssClass="EditForm">
                            <h4>Lookup a user in Active Directory by their user ID<br />
                                <i>NOTE: This is not necessarily their E-mail alias, but the user ID they use to logon to their computer.</i>
                            </h4>
                            <div style="padding:10px;">
                                <label>Enter the user ID:</label> <asp:TextBox ID="tbUserID2" runat="server" Width="100px" MaxLength="20" />
                                <asp:Button ID="btnValidateUser" Text="Find User" runat="server" CommandName="ValidateUser" />
                            </div>
                        </asp:Panel>
                        <asp:Panel ID="pnlInsert2" runat="server" CssClass="EditForm">
                            <table id="tableForm2" cellspacing="5" cellpadding="5" width="600" border="0" rules="none" style="border-collapse: collapse; background: #cfe3db;">
                                <tr>
                                    <td colspan="2">
                                        <h4>Add New User</h4>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width:120px;">
                                        <label>User ID:</label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tbUserID3" runat="server" Width="100px" MaxLength="20" Enabled="false" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>Name to appear on Analysis Notes:</label>
                                    </td>
                                    <td>
                                        <label>First Name:</label> <asp:TextBox ID="tbFirstNm2" runat="server" Width="100px" MaxLength="50" />
                                        <label>Last Name:</label> <asp:TextBox ID="tbLastNm2" runat="server" Width="100px" MaxLength="50" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>Administrator:</label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlAdminLevel2" runat="server" Width="100px" DataTextField="level" DataValueField="level" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>PASS Maintainer Level:</label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlPASSLevel2" runat="server" Width="100px" DataTextField="level" DataValueField="level" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>Safety Approver:</label>
                                    </td>
                                    <td>
                                        <asp:RadioButtonList ID="rblSafetyApprover2" runat="server" RepeatDirection="Horizontal">
                                            <asp:ListItem Text="Yes" Value="yes"></asp:ListItem>
                                            <asp:ListItem Text="No" Value="no"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>Assigned Office:</label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlOffice2" runat="server" Width="300px" DataTextField="office_nm" DataValueField="office_cd" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>Status:</label>
                                    </td>
                                    <td>
                                        <asp:RadioButtonList ID="rblStatus2" runat="server" RepeatDirection="Horizontal">
                                            <asp:ListItem Text="Active" Value="true"></asp:ListItem>
                                            <asp:ListItem Text="Inactive" Value="false"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>Show in Progress Reports:</label>
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="cbReports2" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" colspan="2">
                                        <asp:Button ID="btnInsert" Text="Insert" runat="server" CommandName="PerformInsert" />
                                        <asp:Button ID="btnCancel2" Text="Cancel" runat="server" CausesValidation="false" CommandName="Cancel" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </FormTemplate>
                </EditFormSettings>
                <EditItemStyle BackColor="#cfe3db" />
            </MasterTableView>
            <ClientSettings EnableRowHoverStyle="true" EnablePostBackOnRowClick="true" ClientEvents-OnKeyPress="CheckKey" AllowKeyboardNavigation="true">
                <Selecting AllowRowSelect="True" />
            </ClientSettings>
        </telerik:RadGrid>
    </asp:Panel>
    </telerik:RadAjaxPanel>
</asp:Content>
