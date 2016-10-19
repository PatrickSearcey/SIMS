<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/RMS.Master" CodeBehind="UltDataAging.aspx.vb" Inherits="SIMS.UltDataAging" %>
<%@ MasterType  virtualPath="~/RMS.master" %>
<%@ Register Assembly="SIMSCustomControl" Namespace="SIMSCustomControl" TagPrefix="cc1" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxtoolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" Runat="Server">
<asp:UpdatePanel ID="upSteps" runat="server">
    <ContentTemplate>
        <table width="100%">
            <tr>
                <td valign="top">
                    Choose a WSC: <br />
                    <asp:DropDownList ID="ddlWSC" runat="server" OnSelectedIndexChanged="ddlWSC_SelectedIndexChanged"
                        DataTextField="wsc_nm" DataValueField="wsc_id" AutoPostBack="true" />
                    <asp:Panel ID="pnlOffice" runat="server" Visible="false">
                        Choose an Office: <br />
                        <asp:DropDownList ID="ddlOffice" runat="server" AutoPostBack="true" onSelectedIndexChanged="ddlOffice_SelectedIndexChanged"
                            DataTextField="office_nm" DataValueField="office_id" />
                    </asp:Panel>
                    <asp:Panel ID="pnlOnlyActive" runat="server" Visible="false">Currently viewing: <asp:RadioButtonList ID="rblOnlyActive" runat="server" AutoPostBack="true" 
                            RepeatDirection="Horizontal" OnSelectedIndexChanged="rblOnlyActive_SelectedIndexChanged">
                        <asp:ListItem Value="yes">only active records</asp:ListItem>
                        <asp:ListItem Value="no">all records</asp:ListItem>
                    </asp:RadioButtonList></asp:Panel>
                    <asp:Panel ID="pnlGraph" runat="server" Visible="false">
                        <br /><br /><br />
                        <fieldset style="width:250px;text-align:center;">
                            <legend>Summary Graph</legend>
                            <div style="padding:5px;">
                                <asp:HyperLink ID="hlGraph" runat="server" ImageUrl="images/graphSS.png" />
                            </div>
                        </fieldset>
                    </asp:Panel>
                </td>
                <td valign="top">
                    <asp:Panel ID="pnlExplanation" runat="server" Visible="false">
                        <fieldset style="width:700px;height:260px;font-size:0.88em;color:#666;">
                            <legend>Explanation</legend>
                            <div style="padding:5px;">
                            <div style="width:12px;height:12px;background-color:#f5deb4;border:solid 1px #999966;float:left;"></div>
                            &nbsp;wheat cell backgrounds in the work/check/review period columns mean that the period was completed
                            within the designated category guidelines (within the last 150 days for category 1 records, and within the 
                            last 240 days for category 2 records)<br />
                            <div style="width:12px;height:12px;background-color:#eee8ac;border:solid 1px #999966;float:left;"></div>
                            &nbsp;tan cell backgrounds denote last aging dates within category time limits<br />
                            <div style="width:12px;height:12px;background-color:#beb37b;border:solid 1px #999966;float:left;"></div>
                            &nbsp;olive cell backgrounds signify that all worked periods in RMS have been reviewed<br />
                            <div style="width:12px;height:12px;background-color:#7f8000;border:solid 1px #999966;float:left;"></div>
                            &nbsp;dark olive cell backgrounds signify that all worked periods in RMS have been reviewed within
                            the designated category guidelines (within the last 150 days for category 1 records, and within the last
                            240 days for category 2 records)<br />
                            <b>Notes:</b>
                            <ul style="padding-top:-15px;margin-top:3px;">
                                <li>Clicking on the site number will open the Station Information page</li>
                                <li>Hovering over the record-type code will show the record-type description</li>
                                <li>The <i>cat no</i> column shows the assigned category number for the record.  If category number equal
                                to 1, the last aging dates are highlighted based on being approved in ADAPS within the last 150 days.
                                If category number equal to 2, the last aging dates are highlighted based on being approved in ADAPS within
                                the last 240 days.</li>
                                <li>Clicking on the dates in the last worked, last checked, and last reviewed period in RMS columns will
                                open the corresponding record processing page</li>
                                <li>For cases where only provisional data exists for a DD, the LAST APPROVED DV IN ADAPS columns will be blank.</li>
                            </ul>
                            </div>
                        </fieldset>
                    </asp:Panel>
                </td>
            </tr>
        </table>
        <br />
        <asp:SqlDataSource ID="sdsCRP" runat="server" 
            SelectCommand="SP_CRP_Ult_Data_Aging_Table" SelectCommandType="StoredProcedure"
            OldValuesParameterFormatString="original_{0}" 
            ConnectionString="<%$ ConnectionStrings:simsdbConnectionString %>">
            <SelectParameters>
                <asp:ControlParameter ControlID="ddlOffice" DefaultValue="0" Name="office_id" 
                    PropertyName="SelectedValue" Type="Int32" />
                <asp:ControlParameter ControlID="ddlWSC" DefaultValue="0" Name="wsc_id" 
                    PropertyName="SelectedValue" Type="Int32" />
                <asp:ControlParameter ControlID="rblOnlyActive" DefaultValue="yes" Name="onlyactive" 
                    PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
            <asp:Label runat="server" ID="lblNoResults" CssClass="SIBodyFont" Visible="false" font-bold="true" Font-Italic="true" />
            <div align="right">
            <asp:Label runat="server" ID="lblDateCount" CssClass="SIBodyFont" Visible="false" Font-italic="true" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Label runat="server" ID="lblRecordCount" CssClass="SIBodyFont" Visible="false" Font-Italic="true" /></div>
            <cc1:TwoHeadedGridView ID="gvCRP" runat="server" EnableViewState="true" AutoGenerateColumns="False" 
                DataSourceID="sdsCRP" Width="100%" ShowHeader="true" HeaderText="LAST APPROVED DV IN ADAPS"
                AllowSorting="True" OnDataBound="gvCRP_DataBound" ColumnSpan="2" SecondHeaderStartPos="10">
                <Columns>
                    
                    <asp:BoundField DataField="site_no" ShowHeader="false"
                        SortExpression="site_no" ItemStyle-BackColor="White" ItemStyle-CssClass="hidden" 
                        HeaderStyle-CssClass="hidden" />
                    <asp:BoundField DataField="office_cd" HeaderText="office cd" ItemStyle-BackColor="White"
                        SortExpression="office_cd" ItemStyle-VerticalAlign="Top" />
                    <asp:TemplateField HeaderText="site number" SortExpression="site_no" ItemStyle-BackColor="White" ItemStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <a href='/SIMSClassic/StationInfo.asp?site_no=<%#DataBinder.Eval(Container.DataItem, "site_no").ToString%>' target="_blank">
                            <%#DataBinder.Eval(Container.DataItem, "site_no")%></a>
                        </ItemTemplate>
                    </asp:TemplateField>  
                    <asp:BoundField DataField="station_nm" HeaderText="station name" ItemStyle-Wrap="false" ItemStyle-BackColor="White"
                        SortExpression="station_nm" ItemStyle-VerticalAlign="Top" />
                    <asp:BoundField DataField="dd_nu" HeaderText="DD" SortExpression="dd_nu" ItemStyle-BackColor="White" />
                    <asp:BoundField DataField="parm_cd" HeaderText="param cd" ItemStyle-BackColor="White"
                        SortExpression="parm_cd" />
                    <asp:TemplateField HeaderText="record-type" SortExpression="type_cd" 
                        ItemStyle-BackColor="White">
                        <ItemTemplate>
                            <a href="#" title="<%#DataBinder.Eval(Container.DataItem, "type_ds")%>" 
                                style="color:Black;text-decoration:none;">
                            <%#DataBinder.Eval(Container.DataItem, "type_cd")%></a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="category_no" HeaderText="cat no" ItemStyle-BackColor="White" SortExpression="category_no" />
                    
                    <asp:TemplateField HeaderText="last worked period in RMS" SortExpression="work_period_beg_dt" ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <a href='RecordLists.aspx?src=CRP&listtype=work&office_id=<%#DataBinder.Eval(Container.DataItem, "office_id").ToString%>&rms_record_id=<%#DataBinder.Eval(Container.DataItem, "rms_record_id").ToString%>' target="_blank">
                            <%#DataBinder.Eval(Container.DataItem, "work_period_dt")%></a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:TemplateField HeaderText="last checked period in RMS" SortExpression="check_period_beg_dt" ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <a href='RecordLists.aspx?src=CRP&listtype=check&office_id=<%#DataBinder.Eval(Container.DataItem, "office_id").ToString%>&rms_record_id=<%#DataBinder.Eval(Container.DataItem, "rms_record_id").ToString%>' target="_blank">
                            <%#DataBinder.Eval(Container.DataItem, "check_period_dt")%></a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:TemplateField HeaderText="last reviewed period in RMS" SortExpression="rev_period_beg_dt" ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <a href='RecordLists.aspx?src=CRP&listtype=review&office_id=<%#DataBinder.Eval(Container.DataItem, "office_id").ToString%>&rms_record_id=<%#DataBinder.Eval(Container.DataItem, "rms_record_id").ToString%>' target="_blank">
                            <%#DataBinder.Eval(Container.DataItem, "rev_period_dt")%></a>
                        </ItemTemplate>
                    </asp:TemplateField>
             
                    <asp:BoundField DataField="last_aging_dt" HeaderText="date" HeaderStyle-BackColor="Olive"
                        SortExpression="last_aging_dt" DataFormatString="{0:MM/dd/yy}" htmlencode="false" />
                    <asp:BoundField DataField="DaysSinceAging" HeaderText="days ago" HeaderStyle-BackColor="Olive"
                        SortExpression="DaysSinceAging" ItemStyle-BackColor="White" />
                    
                    <asp:BoundField DataField="work_period_dt" ShowHeader="false" ItemStyle-CssClass="hidden" HeaderStyle-CssClass="hidden" />
                    <asp:BoundField DataField="rev_period_dt" ShowHeader="false" ItemStyle-CssClass="hidden" HeaderStyle-CssClass="hidden" />
                    <asp:BoundField DataField="work_period_end_dt" ShowHeader="false" ItemStyle-CssClass="hidden" HeaderStyle-CssClass="hidden" />
                    <asp:BoundField DataField="check_period_end_dt" ShowHeader="false" ItemStyle-CssClass="hidden" HeaderStyle-CssClass="hidden" />
                    <asp:BoundField DataField="rev_period_end_dt" ShowHeader="false" ItemStyle-CssClass="hidden" HeaderStyle-CssClass="hidden" />
                </Columns>
                <EmptyDataTemplate>
                    <i>No sites returned</i>
                </EmptyDataTemplate>
                <RowStyle Wrap="false" />
                <PagerStyle BackColor="#330066" Font-Bold="True" 
                    Font-Size="Medium" ForeColor="White" />
                <HeaderStyle BackColor="#330066" ForeColor="White" />
                <PagerSettings Mode="NumericFirstLast" />
            </cc1:TwoHeadedGridView>
    </ContentTemplate>
</asp:UpdatePanel>
</asp:Content>
