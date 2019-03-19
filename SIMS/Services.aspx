<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SIMSSiteTopLevel.Master" CodeBehind="Services.aspx.vb" Inherits="SIMS.Services" %>
<%@ MasterType VirtualPath="~/SIMSSiteTopLevel.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManagerProxy ID="ramp" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rgService1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgService1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnSubmit1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgService1" />
                    <telerik:AjaxUpdatedControl ControlID="btnReset1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnReset1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgService1" />
                    <telerik:AjaxUpdatedControl ControlID="btnReset1" />
                    <telerik:AjaxUpdatedControl ControlID="tbSiteNo1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rgService2">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgService2" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnSubmit2">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgService2" />
                    <telerik:AjaxUpdatedControl ControlID="btnReset2" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnReset2">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgService2" />
                    <telerik:AjaxUpdatedControl ControlID="btnReset2" />
                    <telerik:AjaxUpdatedControl ControlID="tbSiteNo2" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rgService3">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgService3" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnSubmit3">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgService3" />
                    <telerik:AjaxUpdatedControl ControlID="btnReset3" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnReset3">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgService3" />
                    <telerik:AjaxUpdatedControl ControlID="btnReset3" />
                    <telerik:AjaxUpdatedControl ControlID="tbSiteNo3" />
                    <telerik:AjaxUpdatedControl ControlID="tbReportType" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManagerProxy>
    <h2>SIMS Services</h2>
    <p>A number of web services are available to give service-oriented application developers access to SIMS data. These services were designed 
    using the <a href="http://msdn.microsoft.com/en-us/library/dd456779.aspx" target="_blank">Windows Communication Foundation (WCF)</a> framework, and 
    can be invoked with the <a href="http://en.wikipedia.org/wiki/REST" target="_blank">REST</a> protocol.</p>

    <p>The WCF URL is below. This should be used in your applications to connect to the services. A description of each of the currently offered services is
    in the next section.</p>

    <p style="font-weight:bold;">
    https://sims.water.usgs.gov/Services/WCFServices.SIMSService.svc?wsdl
    </p>
    <br />
    <hr />

    <h3>Descriptions and Examples</h3>
    
    <fieldset>
        <legend>Get SIMS Site Information for One Site</legend>
        <div style="padding:10px;">
            <p><b>Service Name:</b> GetSiteByNumber(string site_no)</p>
            <p>This service returns the following information for one site (currently only sites on the TX SIMS server can be returned):</p>
            <ul>
                <li>ID (SIMS site ID)</li>
                <li>NWISWebSiteID</li>
                <li>AgencyCd</li>
                <li>Number</li>
                <li>NWISHost</li>
                <li>Name (published station name)</li>
                <li>OfficeID</li>
                <li>OfficeCd</li>
                <li>OfficeName</li>
                <li>DistrictAbbrev</li>
                <li>DistrictName</li>
            </ul>
            <h5>Test the service</h5>
            <br />
            <label>Enter a site number:</label> <asp:TextBox ID="tbSiteNo1" runat="server" />
            <asp:Button ID="btnSubmit1" runat="server" Text="Go!" OnClick="btnSubmit1_Click" />
            <br /><br />
            <telerik:RadGrid ID="rgService1" runat="server" Skin="Web20" Visible="false" AutoGenerateColumns="false">
                <MasterTableView AllowSorting="false" AllowFilteringByColumn="false">
                    <Columns>
                        <telerik:GridBoundColumn DataField="ID" HeaderText="ID" />
                        <telerik:GridBoundColumn DataField="NWISWebSiteID" HeaderText="NWISWebSiteID" />
                        <telerik:GridBoundColumn DataField="AgencyCd" HeaderText="AgencyCd" />
                        <telerik:GridBoundColumn DataField="Number" HeaderText="Number" />
                        <telerik:GridBoundColumn DataField="NWISHost" HeaderText="NWISHost" />
                        <telerik:GridBoundColumn DataField="Name" HeaderText="Name" />
                        <telerik:GridBoundColumn DataField="OfficeID" HeaderText="OfficeID" />
                        <telerik:GridBoundColumn DataField="OfficeCd" HeaderText="OfficeCd" />
                        <telerik:GridBoundColumn DataField="OfficeName" HeaderText="OfficeName" />
                        <telerik:GridBoundColumn DataField="DistrictAbbrev" HeaderText="DistrictAbbrev" />
                        <telerik:GridBoundColumn DataField="DistrictName" HeaderText="DistrictName" />
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
            <br />
            <asp:Button ID="btnReset1" runat="server" Text="Reset and clear grid" OnClick="btnReset1_Click" Visible="false" />
        </div>
    </fieldset>
    <br /><br />
    <fieldset>
        <legend>Get All Elements for One Site</legend>
        <div style="padding:10px;">
            <p><b>Service Name:</b> GetElementsBySite(string site_no)</p>
            <p>This service returns the following element information for one site (currently only sites on the TX SIMS server can be returned):</p>
            <ul>
                <li>ElementName</li>
                <li>ElementInfo</li>
                <li>RevisedBy</li>
                <li>RevisedDate</li>
                <li>Remarks</li>
                <li>ReportTypeCd</li>
            </ul>
            <h5>Test the service</h5>
            <br />
            <label>Enter a site number:</label> <asp:TextBox ID="tbSiteNo2" runat="server" />
            <asp:Button ID="btnSubmit2" runat="server" Text="Go!" OnClick="btnSubmit2_Click" />
            <br /><br />
            <telerik:RadGrid ID="rgService2" runat="server" Skin="Web20" Visible="false" AutoGenerateColumns="false">
                <MasterTableView AllowSorting="false" AllowFilteringByColumn="false">
                    <Columns>
                        <telerik:GridBoundColumn DataField="ElementName" HeaderText="ElementName" />
                        <telerik:GridBoundColumn DataField="ElementInfo" HeaderText="ElementInfo" />
                        <telerik:GridBoundColumn DataField="RevisedBy" HeaderText="RevisedBy" />
                        <telerik:GridBoundColumn DataField="RevisedDate" HeaderText="RevisedDate" />
                        <telerik:GridBoundColumn DataField="ReportTypeCd" HeaderText="ReportTypeCd" />
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
            <br />
            <asp:Button ID="btnReset2" runat="server" Text="Reset and clear grid" OnClick="btnReset2_Click" Visible="false" />
        </div>
    </fieldset>
    <br /><br />
    <fieldset>
        <legend>Get All Elements for One Site</legend>
        <div style="padding:10px;">
            <p><b>Service Name:</b> GetElementsBySite(string site_no)</p>
            <p>This service returns the following element information for one site (currently only sites on the TX SIMS server can be returned):</p>
            <ul>
                <li>ElementName</li>
                <li>ElementInfo</li>
                <li>RevisedBy</li>
                <li>RevisedDate</li>
                <li>Remarks</li>
                <li>ReportTypeCd</li>
            </ul>
            <h5>Test the service</h5>
            <br />
            <label>Enter a site number:</label> <asp:TextBox ID="tbSiteNo3" runat="server" /> <asp:TextBox ID="tbAgencyCd3" runat="server" Text="USGS" Width="50px" />
            <label>Enter a report type code (MANU, SDESC, SANAL):</label> <asp:TextBox ID="tbReportType" runat="server" />
            <asp:Button ID="btnSubmit3" runat="server" Text="Go!" OnClick="btnSubmit3_Click" />
            <br /><br />
            <telerik:RadGrid ID="rgService3" runat="server" Skin="Web20" Visible="false" AutoGenerateColumns="false">
                <MasterTableView AllowSorting="false" AllowFilteringByColumn="false">
                    <Columns>
                        <telerik:GridBoundColumn DataField="ElementName" HeaderText="ElementName" />
                        <telerik:GridBoundColumn DataField="ElementInfo" HeaderText="ElementInfo" />
                        <telerik:GridBoundColumn DataField="RevisedBy" HeaderText="RevisedBy" />
                        <telerik:GridBoundColumn DataField="RevisedDate" HeaderText="RevisedDate" />
                        <telerik:GridBoundColumn DataField="ReportTypeCd" HeaderText="ReportTypeCd" />
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
            <br />
            <asp:Button ID="btnReset3" runat="server" Text="Reset and clear grid" OnClick="btnReset3_Click" Visible="false" />
        </div>
    </fieldset>
</asp:Content>
