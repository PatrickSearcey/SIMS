<%@ Page Title="" Language="C#" MasterPageFile="~/SIMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="Archive.aspx.cs" Inherits="SIMS2017.StationDoc.Archive" %>
<%@ Register Src="~/Control/SitePageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../styles/stationdoc.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="gvElementList">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlStep1" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlStep2" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnBack1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlStep2" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlStep1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnBack2">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlStep3" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlStep2" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnBack3">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlStep3" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlStep1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnRetrieve">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlStep2" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlStep3" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap" />

    <uc:PageHeading id="ph1" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
    <div class="mainContent">
        <asp:Panel ID="pnlStep1" runat="server">
            <h5>Begin by choosing an element from the list of possible choices:</h5>
            <asp:GridView ID="gvElementList" runat="server" AutoGenerateColumns="False" BorderColor="#314d93" BorderWidth="1px" 
                CellPadding="5" GridLines="Horizontal" OnDataBound="gvElementList_Bound" AllowSorting="true">
                <Columns>
                    <asp:TemplateField ShowHeader="False">
                        <ItemTemplate>
                            <asp:LinkButton ID="lbSelectElement" runat="server" CausesValidation="False" 
                                CommandName='<%# DataBinder.Eval(Container.DataItem,"element_id") %>' 
                                Text="Select" OnCommand="ElementSelected"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="element_nm" HeaderText="Element Name" SortExpression="element_nm" />
                    <asp:BoundField DataField="first_revised" HeaderText="Earliest Revised Date" SortExpression="first_revised" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="last_revised" HeaderText="Last Revised Date" SortExpression="last_revised" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="revisions" HeaderText="No. of Revisions" SortExpression="revisions" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="priority" ShowHeader="false" ItemStyle-CssClass="hidden" HeaderStyle-CssClass="hidden" />
                </Columns>
                <EmptyDataTemplate>
                    <i>No elements have been archived yet.</i>
                </EmptyDataTemplate>
                <HeaderStyle Font-Bold="True" ForeColor="#d314d93" />
            </asp:GridView>
            <p style="font-weight:bold;">Note:&nbsp;&nbsp;SDESC, 
            <img src="../images/keycolor0.gif" alt="white color" /> = Station Description;
            &nbsp;&nbsp;SANAL, <img src="../images/keycolor1.gif" alt="gray color" /> = Station Analysis;
            &nbsp;&nbsp;MANU, <img src="../images/keycolor2.gif" alt="purple color" /> = Manuscripts</p>
        </asp:Panel>
        <asp:Panel ID="pnlStep2" runat="server">
            <h5>You may choose to modify the begin and end dates below for the archived information retrieval.</h5>
            <p>Click the Retrieve button when ready.</p>
            <p>Chosen element: <asp:Label ID="lblElementName" runat="server" Font-Bold="true" ForeColor="Red" /><br /><br />

            <asp:TextBox ID="tbBeginDate" runat="server" Width="80" />
            <asp:CompareValidator ID="cvBeginDate" Display="dynamic" ControlToValidate="tbBeginDate" Type="Date" Operator="DataTypeCheck" Text="*" ErrorMessage="Please enter a valid begin date" runat="server" />
            <asp:RequiredFieldValidator ID="rfvBeginDate" runat="server" ErrorMessage="You must enter a begin date" Text="*" ControlToValidate="tbBeginDate"/>
            to &nbsp;&nbsp;
            <asp:TextBox ID="tbEndDate" runat="server" Width="80" />
            <asp:CompareValidator ID="cvEndDate" Display="dynamic" ControlToValidate="tbEndDate" Type="Date" Operator="DataTypeCheck" Text="*" ErrorMessage="Please enter a valid end date" runat="server" />
            <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ErrorMessage="You must enter an end date" Text="*" ControlToValidate="tbEndDate" />
            
            <asp:ValidationSummary ID="ValSummary" FormToValidate="form1" runat="server" /></p>

            <telerik:RadButton ID="btnBack1" runat="server" Text="&laquo; Back" CausesValidation="false" OnCommand="Back_Command" CommandName="back1" Skin="Bootstrap" />
            <telerik:RadButton ID="btnRetrieve" runat="server" Text="Retrieve" Skin="Bootstrap" OnCommand="RetrieveArchivedInfo" />
        </asp:Panel>
        
        <asp:Panel ID="pnlStep3" runat="server">
            <asp:LinkButton ID="lbBack3" runat="server" Text="&laquo; choose a different element" OnCommand="Back_Command" CommandName="back3" /> &nbsp;&nbsp;
            <asp:LinkButton ID="lbBack2" runat="server" Text="&laquo; modify dates" OnCommand="Back_Command" CommandName="back2" />
            
            <h4><asp:Label ID="lblElementName2" runat="server" /></h4>
            <asp:DataList ID="dlElementInfo" runat="server">
                <ItemTemplate>
                    <p><b>Revised By:</b> <%# DataBinder.Eval(Container.DataItem, "userid") %> &nbsp;&nbsp;&nbsp;
                    <b>Revised Date:</b> <%#DataBinder.Eval(Container.DataItem, "date")%></p>
                    <p><%#DataBinder.Eval(Container.DataItem, "element_info")%></p>
                    <hr />
                </ItemTemplate>
            </asp:DataList>
            <asp:Label ID="lblNothingReturned" runat="server" Visible="false" Text="No results returned. Please modify dates and try again." />
        </asp:Panel>
    </div>
</asp:Content>
