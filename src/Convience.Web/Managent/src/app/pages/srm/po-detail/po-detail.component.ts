import { Component, OnInit,ViewEncapsulation } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { SrmPoService } from '../../../business/srm/srm-po.service';
import { SrmDeliveryService } from '../../../business/srm/srm-delivery.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { DateFilterModel } from 'ag-grid-community';
import { Router,ActivatedRoute } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { StorageService } from 'src/app/services/storage.service';
@Component({
  selector: 'app-po-detail',
  encapsulation: ViewEncapsulation.None,
  templateUrl: './po-detail.component.html',
  styleUrls: ['./po-detail.component.less']
})
export class PoDetailComponent implements OnInit {
  gridApi;
  gridColumnApi;

  columnDefs;
  autoGroupColumnDef;
  defaultColDef;
  rowSelection;
  rowGroupPanelShow;
  pivotPanelShow;
  paginationPageSize;
  paginationNumberFormatter;
  rowData;
  searchForm: FormGroup = new FormGroup({});
  deliverydate;
  vendorsn;
  manager;
  isSpinning = false;
  constructor(private _formBuilder: FormBuilder,private http: HttpClient,private _srmPoService: SrmPoService,private _srmDeliveryService: SrmDeliveryService
    ,private router: Router,private _messageService: NzMessageService,private _storageService: StorageService,) {
    this.columnDefs = [
      {
        headerName:'料號',
        field: 'Matnr',
        checkboxSelection: checkboxSelection,
        headerCheckboxSelection: headerCheckboxSelection,
      },
      {
        headerName:'物料內文',
        field: 'Description',
      },
      {
        headerName:'採購單號',
        field: 'PoNum',
        minWidth: 170,

      },
      {
        headerName:'採購單識別碼',
        field: 'PoId',
        minWidth: 170,
        hide:'true',
      },
      {
        headerName:'項次',
        field: 'PoLId',
      },
      {
        headerName:'採單數量',
        field: 'Qty',
      },
      {
        headerName:'未交貨數量',
        field: 'RemainQty',
      },      
      {
        headerName:'此次交貨數量',
        field: 'DeliveryQty',
        editable:true,
        valueGetter: function (params) {
          if(params.data.DeliveryQty>params.data.RemainQty)
          {
            params.data.DeliveryQty=params.data.RemainQty;
          }
          return params.data.DeliveryQty;
          // if (params.node.group) {
          //   return params.node.key;
          // } else {
          //   return params.data[params.colDef.field];
          // }
        },
        cellStyle:{background: '#f0f8ff'},
      },
      {
        headerName:'工單可交數',
        field: 'WoQty',
      },
      {
        headerName:'單價',
        field: 'Price',
      },
      {
        headerName:'本次需求日',
        field: 'DeliveryDate',
        valueFormatter:dateFormatter
      },
      {
        headerName:'廠商交貨日期',
        field: 'ReplyDeliveryDate',
        valueFormatter:dateFormatter,
      },
      {
        headerName:'倉別',
        field: 'Storage',
        valueGetter: function (params) {
          if(params.data.Storage=='05Z1')
          {
            return '服務倉'
          }
          return '一般倉';
        },
      },
      {
        headerName:'供應商識別碼',
        field: 'VendorId',
        hide:'true',
      },
      {
        headerName:'供應商',
        field: 'VendorName',
      },
      {
        headerName:'採購員',
        field: 'EkgryDesc',
      },
      {
        headerName:'狀態',
        field: 'StatusDesc',
      },

      {
        headerName:'採購單總金額',
        field: 'TotalAmount',
        hide:'true'
      },
      {
        headerName:'料號識別碼',
        field: 'MatnrId',
        hide:'true'
      },
      {
        headerName:'交貨地點',
        field: 'DeliveryPlace',
        hide:'true'
      },
      {
        headerName:'關鍵零組件',
        field: 'CriticalPart',
        hide:'true'
      },
      {
        headerName:'檢驗時間(天)',
        field: 'InspectionTime',
        hide:'true'
      },
    ];
    this.autoGroupColumnDef = {
      headerName: 'Group',
      minWidth: 170,
      field: 'athlete',
      valueGetter: function (params) {
        if (params.node.group) {
          return params.node.key;
        } else {
          return params.data[params.colDef.field];
        }
      },
      headerCheckboxSelection: true,
      cellRenderer: 'agGroupCellRenderer',
      cellRendererParams: { checkbox: true },
    };
    this.defaultColDef = {
      editable: false,
      enableRowGroup: true,
      enablePivot: true,
      enableValue: true,
      sortable: true,
      resizable: true,
      filter: true,
      flex: 1,
      minWidth: 150,
    };
    this.rowSelection = 'multiple';
    this.rowGroupPanelShow = 'always';
    this.pivotPanelShow = 'always';
    this.paginationPageSize = 15;
    this.paginationNumberFormatter = function (params) {
      return '[' + params.value.toLocaleString() + ']';
    };
  }

  ngOnInit(): void {
    var Werks= this._storageService.werks.split(',');
    var werkselected=null;
    if(Werks.length==1)
    {
      werkselected=Werks[0];
    }
    this.searchForm = this._formBuilder.group({
      PO_NUM: [null],
      STATUS: [1],
      EkgryDesc:[null],
      replyDeliveryDate_s: [null],
      replyDeliveryDate_e: [null],
      ORG: [werkselected],
    });
  }
  // onPageSizeChanged(newPageSize) {
  //   var value = document.getElementById('page-size').value;
  //   this.gridApi.paginationSetPageSize(Number(value));
  // }

  onGridReady(params) {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;
    this.rowData =null;
    var Werks= this._storageService.werks.split(',');
    if(Werks.length==1)
    {
      //this.gridApi.showNoRowsOverlay();
      this.refresh();
    }
    //
    //
  }
  submitSearch() {
    this.refresh();
  }
  pageChange() {
    this.refresh();
  }
  refresh() {
    if(this.searchForm.value["ORG"] == null)
    {
      this._messageService.warning(`請先選擇廠區.`);
    }
    else
    {
      var query = {
        poNum: this.searchForm.value["PO_NUM"] == null ? "" : this.searchForm.value["PO_NUM"],
        status: "15",
        replyDeliveryDate_s: this.searchForm.value["ReplyDeliveryDate_s"] == null ? "" : this.searchForm.value["ReplyDeliveryDate_s"],
        replyDeliveryDate_e: this.searchForm.value["ReplyDeliveryDate_e"] == null ? "" : this.searchForm.value["ReplyDeliveryDate_e"],
        org: this.searchForm.value["ORG"] == null ? "" : this.searchForm.value["ORG"],
      }
      this.getPoLList(query);
    }

  }
  getPoLList(query){
    if(query==null)
    {
      query = {
        poNum: "",
        status: "15",
        replyDeliveryDate_s: null,
        replyDeliveryDate_e: null,
      }
    }
    this._srmPoService.GetPoL(query)
      .subscribe((result) => {
        this.rowData = result;
        console.info()
      });
  }
  getSelectedRowData(event) {
    let selectedNodes = this.gridApi.getSelectedNodes();
    if(selectedNodes.length==0)
    {
      alert('請先選擇要出貨的項目!');
      return;
    }
    let selectedData = selectedNodes.map(node => node.data);
    var query = {
      date: this.deliverydate,
      data: selectedData,
      vendorsn: this.vendorsn,
      manager: this.manager,
    }
    console.info(query);
    this._srmDeliveryService.AddDelivery(query)
    .subscribe((result:any) => {
      if(result!=null) alert('出貨單生成成功');
      this.router.navigate(['/srm/deliveryl/'+result]);
    });
  }
  sapDelivery(event) {
    this.isSpinning=true;
    this._srmDeliveryService.SapDelivery()
    .subscribe((result:any) => {
      this.isSpinning=false;
      if(result==null) alert('重新生成成功');
      else
      {
       alert(result);
      }
    });
  }
  cancel()
  {}
}
var checkboxSelection = function (params) {
  return params.columnApi.getRowGroupColumns().length === 0;
};
var headerCheckboxSelection = function (params) {
  return params.columnApi.getRowGroupColumns().length === 0;
};
function dateFormatter(data) {
  if(data.value==null) return "";
  var date=new Date(data.value);
  return `${date.getFullYear()}-${date.getMonth()+1}-${date.getDate()}`;
}
