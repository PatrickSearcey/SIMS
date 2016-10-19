<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/RMS.Master" CodeBehind="RecordTypeConfig.aspx.vb" Inherits="SIMS.RecordTypeConfig" ValidateRequest="false" %>
<%@ MasterType  virtualPath="~/RMS.master" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxtoolkit" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<style type="text/css">
    .EditFormHeader td
    {
        background: white;
        padding: 5px 0px;
    }
    .MyImageButton
    {
       cursor: hand;
    }
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" Runat="Server">
<asp:Panel ID="pnlHasAccess" runat="server">
    <telerik:RadAjaxPanel ID="rapMain" runat="server">
        <asp:Literal ID="ltlTest" runat="server" />
        <telerik:RadGrid ID="rgRecordTypes" runat="server" Width="850px" GridLines="None" AllowPaging="True" PageSize="20" Skin="Sunset"
            AllowSorting="True" AutoGenerateColumns="False" ShowStatusBar="true" OnLoad="rgRecordTypes_Load">
            <MasterTableView Width="100%" EditMode="PopUp"
                CommandItemSettings-AddNewRecordText="Add new record-type" DataKeyNames="record_type_id">
                <Columns>
                    <telerik:GridEditCommandColumn ButtonType="ImageButton" UniqueName="EditCommandColumn" HeaderText="Edit Record">
                        <HeaderStyle Wrap="true"></HeaderStyle>
                        <ItemStyle CssClass="MyImageButton" Width="5%" />
                    </telerik:GridEditCommandColumn>
                    <telerik:GridBoundColumn UniqueName="type_cd" HeaderText="Type Code" DataField="type_cd">
                        <HeaderStyle Wrap="false"></HeaderStyle>
                        <ItemStyle Width="15%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="type_ds" HeaderText="Type Description" DataField="type_ds">
                        <ItemStyle Width="65%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn HeaderText="Time-Series" SortExpression="ts_fg" UniqueName="ts_fg">
                        <ItemTemplate>
                            <asp:Image ID="imgContorNoncont" runat="server" ImageUrl='<%# "images/" & Eval("ts_fg") & ".png" %>' />
                        </ItemTemplate>
                        <ItemStyle Width="15%" HorizontalAlign="Center" />
                    </telerik:GridTemplateColumn>
                </Columns>
                <EditFormSettings UserControlName="Controls/RecordTypeConfigDetails.ascx" EditFormType="WebUserControl">
                    <EditColumn UniqueName="EditCommandColumn1">
                    </EditColumn>
                    <PopUpSettings Width="700px" />
                </EditFormSettings>
                <ExpandCollapseColumn ButtonType="ImageButton" Visible="False" UniqueName="ExpandColumn">
                    <HeaderStyle Width="19px"></HeaderStyle>
                </ExpandCollapseColumn>
            </MasterTableView>
        </telerik:RadGrid>
    </telerik:RadAjaxPanel>
</asp:Panel>
</asp:Content>
