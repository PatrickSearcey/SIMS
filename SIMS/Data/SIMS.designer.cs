﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Data
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="simsdb")]
	public partial class SIMSDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertSite(Site instance);
    partial void UpdateSite(Site instance);
    partial void DeleteSite(Site instance);
    #endregion
		
		public SIMSDataContext() : 
				base(global::Data.Properties.Settings.Default.simsdbConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public SIMSDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public SIMSDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public SIMSDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public SIMSDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<Site> Sites
		{
			get
			{
				return this.GetTable<Site>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.SIMS_Site_Master")]
	public partial class Site : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _site_id;
		
		private System.Nullable<decimal> _nwisweb_site_id;
		
		private string _agency_cd;
		
		private string _site_no;
		
		private string _nwis_host;
		
		private string _db_no;
		
		private string _station_full_nm;
		
		private System.Nullable<int> _office_id;
		
		private string _alt_basin_nm;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void Onsite_idChanging(int value);
    partial void Onsite_idChanged();
    partial void Onnwisweb_site_idChanging(System.Nullable<decimal> value);
    partial void Onnwisweb_site_idChanged();
    partial void Onagency_cdChanging(string value);
    partial void Onagency_cdChanged();
    partial void Onsite_noChanging(string value);
    partial void Onsite_noChanged();
    partial void Onnwis_hostChanging(string value);
    partial void Onnwis_hostChanged();
    partial void Ondb_noChanging(string value);
    partial void Ondb_noChanged();
    partial void Onstation_full_nmChanging(string value);
    partial void Onstation_full_nmChanged();
    partial void Onoffice_idChanging(System.Nullable<int> value);
    partial void Onoffice_idChanged();
    partial void Onalt_basin_nmChanging(string value);
    partial void Onalt_basin_nmChanged();
    #endregion
		
		public Site()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_site_id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int site_id
		{
			get
			{
				return this._site_id;
			}
			set
			{
				if ((this._site_id != value))
				{
					this.Onsite_idChanging(value);
					this.SendPropertyChanging();
					this._site_id = value;
					this.SendPropertyChanged("site_id");
					this.Onsite_idChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_nwisweb_site_id", DbType="Decimal(10,0)")]
		public System.Nullable<decimal> nwisweb_site_id
		{
			get
			{
				return this._nwisweb_site_id;
			}
			set
			{
				if ((this._nwisweb_site_id != value))
				{
					this.Onnwisweb_site_idChanging(value);
					this.SendPropertyChanging();
					this._nwisweb_site_id = value;
					this.SendPropertyChanged("nwisweb_site_id");
					this.Onnwisweb_site_idChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_agency_cd", DbType="NVarChar(5) NOT NULL", CanBeNull=false)]
		public string agency_cd
		{
			get
			{
				return this._agency_cd;
			}
			set
			{
				if ((this._agency_cd != value))
				{
					this.Onagency_cdChanging(value);
					this.SendPropertyChanging();
					this._agency_cd = value;
					this.SendPropertyChanged("agency_cd");
					this.Onagency_cdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_site_no", DbType="NVarChar(15) NOT NULL", CanBeNull=false)]
		public string site_no
		{
			get
			{
				return this._site_no;
			}
			set
			{
				if ((this._site_no != value))
				{
					this.Onsite_noChanging(value);
					this.SendPropertyChanging();
					this._site_no = value;
					this.SendPropertyChanged("site_no");
					this.Onsite_noChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_nwis_host", DbType="NVarChar(12) NOT NULL", CanBeNull=false)]
		public string nwis_host
		{
			get
			{
				return this._nwis_host;
			}
			set
			{
				if ((this._nwis_host != value))
				{
					this.Onnwis_hostChanging(value);
					this.SendPropertyChanging();
					this._nwis_host = value;
					this.SendPropertyChanged("nwis_host");
					this.Onnwis_hostChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_db_no", DbType="NVarChar(2) NOT NULL", CanBeNull=false)]
		public string db_no
		{
			get
			{
				return this._db_no;
			}
			set
			{
				if ((this._db_no != value))
				{
					this.Ondb_noChanging(value);
					this.SendPropertyChanging();
					this._db_no = value;
					this.SendPropertyChanged("db_no");
					this.Ondb_noChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_station_full_nm", DbType="NVarChar(150)")]
		public string station_full_nm
		{
			get
			{
				return this._station_full_nm;
			}
			set
			{
				if ((this._station_full_nm != value))
				{
					this.Onstation_full_nmChanging(value);
					this.SendPropertyChanging();
					this._station_full_nm = value;
					this.SendPropertyChanged("station_full_nm");
					this.Onstation_full_nmChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_office_id", DbType="Int")]
		public System.Nullable<int> office_id
		{
			get
			{
				return this._office_id;
			}
			set
			{
				if ((this._office_id != value))
				{
					this.Onoffice_idChanging(value);
					this.SendPropertyChanging();
					this._office_id = value;
					this.SendPropertyChanged("office_id");
					this.Onoffice_idChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_alt_basin_nm", DbType="NVarChar(150)")]
		public string alt_basin_nm
		{
			get
			{
				return this._alt_basin_nm;
			}
			set
			{
				if ((this._alt_basin_nm != value))
				{
					this.Onalt_basin_nmChanging(value);
					this.SendPropertyChanging();
					this._alt_basin_nm = value;
					this.SendPropertyChanged("alt_basin_nm");
					this.Onalt_basin_nmChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
