<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SIMSSite.Master" CodeBehind="Archives.aspx.vb" Inherits="SIMS.Archives" %>
<%@ MasterType  virtualPath="~/SIMSSite.master" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxtoolkit" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" Runat="Server">
<asp:UpdatePanel ID="upSteps" runat="server">
    <ContentTemplate>
        <asp:Panel ID="pnlStep1" runat="server" BackColor="#f0f0f0">
            <asp:Label ID="lblTest" runat="server" />
            <p style="padding-left:10px;padding-right:10px;font-weight:bold;margin-top:0px;">Begin by choosing an element from the list of 
            possible choices:</p>
            <div style="padding-left:15px;">
            <asp:GridView ID="gvElementList" runat="server" 
                AutoGenerateColumns="False" DataSourceID="sdsElementList" BackColor="White" 
                    BorderColor="#E7E7FF" BorderStyle="None" BorderWidth="1px" CellPadding="3" 
                    GridLines="Horizontal" OnDataBound="gvElementList_Bound">
                <FooterStyle BackColor="#B5C7DE" ForeColor="#4A3C8C" />
                <RowStyle BackColor="#9999CC" ForeColor="#4A3C8C" />
                <Columns>
                    <asp:TemplateField ShowHeader="False">
                        <ItemTemplate>
                            <asp:LinkButton ID="lbSelectElement" runat="server" CausesValidation="False" 
                                CommandName='<%# DataBinder.Eval(Container.DataItem,"element_id") %>' 
                                Text="Select" OnCommand="ElementSelected"></asp:LinkButton>
                        </ItemTemplate>
                        <ItemStyle ForeColor="Red" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="element_nm" HeaderText="Element Name" 
                        SortExpression="element_nm" />
                    <asp:BoundField DataField="first_revised" HeaderText="Earliest Revised Date" 
                        ReadOnly="True" SortExpression="first_revised" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="last_revised" HeaderText="Last Revised Date" 
                        ReadOnly="True" SortExpression="last_revised" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="revisions" HeaderText="No. of Revisions" 
                        ReadOnly="True" SortExpression="revisions" 
                        ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="priority" ShowHeader="false" ItemStyle-CssClass="hidden"
                        HeaderStyle-CssClass="hidden" />
                </Columns>
                <EmptyDataTemplate>
                    <i>No elements have been archived yet.</i>
                </EmptyDataTemplate>
                <PagerStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" HorizontalAlign="Right" />
                <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="#F7F7F7" />
                <HeaderStyle BackColor="#324d92" Font-Bold="True" ForeColor="#F7F7F7" />
            </asp:GridView></div>
            <asp:SqlDataSource ID="sdsElementList" runat="server" 
                ConnectionString="<%$ ConnectionStrings:simsdbConnectionString %>" 
                SelectCommand="SP_Element_Info_Archives" SelectCommandType="StoredProcedure">
                <SelectParameters>
                    <asp:QueryStringParameter DefaultValue="0" Name="site_id" 
                        QueryStringField="site_id" Type="Int32" />
                </SelectParameters>
            </asp:SqlDataSource>
            <p style="padding-left:10px;padding-right:10px;font-weight:bold;margin-bottom:0px;">Note:&nbsp;&nbsp;SDESC, 
            <img src="images/keycolor0.gif" alt="white color" /> = Station Description;
            &nbsp;&nbsp;SANAL, <img src="images/keycolor1.gif" alt="gray color" /> = Station Analysis;
            &nbsp;&nbsp;MANU, <img src="images/keycolor2.gif" alt="purple color" /> = Manuscripts</p>
        </asp:Panel>
        
        <asp:Panel ID="pnlStep2" runat="server" BackColor="#f0f0f0" Visible="false">
            <p style="padding-left:10px;padding-right:10px;font-weight:bold;margin-top:0px;">You may 
            choose to modify the begin and end dates below for the archived information retrieval.<br />
            Click the Retrieve button when ready.</p>
            <p style="padding-left:10px;padding-right:10px;">
            Chosen element: <asp:Label ID="lblElementName" runat="server" Font-Bold="true" ForeColor="Red" /><br /><br />
            <asp:TextBox ID="tbBeginDate" runat="server" Width="80" />
            
            <asp:CompareValidator ID="cvBeginDate" Display="dynamic" ControlToValidate="tbBeginDate"
                Type="Date" Operator="DataTypeCheck" Text="*" ErrorMessage="Please enter a valid begin date" runat="server" />
            <asp:RequiredFieldValidator ID="rfvBeginDate" runat="server" 
                ErrorMessage="You must enter a begin date" Text="*" ControlToValidate="tbBeginDate"/>
            
            to &nbsp;&nbsp;
            <asp:TextBox ID="tbEndDate" runat="server" Width="80" />
            
            <asp:CompareValidator ID="cvEndDate" Display="dynamic" ControlToValidate="tbEndDate"
                Type="Date" Operator="DataTypeCheck" Text="*" ErrorMessage="Please enter a valid end date" runat="server" />
            <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" 
                ErrorMessage="You must enter an end date" Text="*" ControlToValidate="tbEndDate" />
            
            <asp:ValidationSummary ID="ValSummary" FormToValidate="form1" runat="server" />
            </p>
            <p style="padding-left:10px;padding-right:10px;margin-bottom:0px;">
            <asp:Button id="btnBack1" runat="server" Text="&laquo; Back" CausesValidation="false"
                OnCommand="Back_Command" CommandName="back1" />
            <asp:Button ID="btnRetrieve" runat="server" Text="Retrieve"
                OnCommand="RetrieveArchivedInfo" /></p>
        </asp:Panel>
        
        <asp:Panel ID="pnlStep3" runat="server" Visible="false">
            <asp:LinkButton ID="lbBack3" runat="server" Text="&laquo; choose a different element" 
                OnCommand="Back_Command" CommandName="back3" /> &nbsp;&nbsp;
            <asp:LinkButton ID="lbBack2" runat="server" Text="&laquo; modify dates" 
                OnCommand="Back_Command" CommandName="back2" />
            
            <p class="SITitleFontSmall">
            <asp:Label ID="lblElementName2" runat="server" /></p>
            <asp:Repeater ID="rptElementInfo" runat="server">
                <ItemTemplate>
                    <p style="padding-left:10px;padding-right:10px;">
                    <b>Revised By:</b> 
                    <%#DataBinder.Eval(Container.DataItem, "userid")%> &nbsp;&nbsp;&nbsp;
                    <b>Revised Date:</b> <%#DataBinder.Eval(Container.DataItem, "date")%>
                    </p>
                    <p style="padding-left:10px;padding-right:10px;">
                    <%#DataBinder.Eval(Container.DataItem, "element_info")%>
                    </p>
                    <hr />
                </ItemTemplate>
            </asp:Repeater>
            <asp:Label ID="lblNothingReturned" runat="server" Visible="false" 
                Text="No results returned. Please modify dates and try again." />
        </asp:Panel>
        
        <ajaxToolkit:RoundedCornersExtender ID="rce1" runat="server" 
            TargetControlID="pnlStep1" 
            BorderColor="#390079" 
            Enabled="True" Radius="15" />
        <ajaxToolkit:RoundedCornersExtender ID="rce2" runat="server" 
            TargetControlID="pnlStep2" 
            BorderColor="#390079" 
            Enabled="True" Radius="15" />
         
    </ContentTemplate>
</asp:UpdatePanel>
</asp:Content>
