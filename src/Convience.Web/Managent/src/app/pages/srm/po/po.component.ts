import { Component, OnInit,ViewEncapsulation } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NzDatePickerComponent} from 'ng-zorro-antd/date-picker';
import datepickerFactory from 'jquery-datepicker';
import datepickerJAFactory from 'jquery-datepicker/i18n/jquery.ui.datepicker-en-GB';
import { AgGridDatePickerComponent} from './AGGridDatePickerCompponent';
import { SrmPoService } from '../../../business/srm/srm-po.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
// import { TotalValueRenderer } from './total-value-renderer.component';
declare const $: any; // avoid the error on $(this.eInput).datepicker();
datepickerFactory($);
datepickerJAFactory($);
@Component({
  selector: 'app-po',
  encapsulation: ViewEncapsulation.None,
  templateUrl: './po.component.html',
  styleUrls: ['./po.component.less']
})
export class PoComponent implements OnInit {
  gridApi;
  gridColumnApi;
  columnDefs;
  defaultColDef;
  detailCellRendererParams;
  components;
  rowData: any;
  searchForm: FormGroup = new FormGroup({});
  page: number = 1;
  size: number = 2;
  total: number;
  constructor(private _formBuilder: FormBuilder,private http: HttpClient,private _srmPoService: SrmPoService) {
    this.columnDefs = [
      {
        headerName:'採購單識別碼',
        field: 'PoId',
        cellRenderer: 'agGroupCellRenderer',
      },
      {
        headerName:'採購單號',
        field: 'PoNum',
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
        headerName:'狀態',
        field: 'Status',
        cellClassRules: {
          'rag-green': 'x == 12',
          'rag-amber': 'x == 11',
          'rag-red': 'x == 10',
        },
        valueFormatter:'switch(value){case 10 : return "待確認"; case 11 : return "已確認"; case 12 : return "已回覆"; default : return "未知";}'
      },
      {
        headerName:'採購單總金額',
        field: 'TotalAmount',
      },
      {
        headerName:'採購人員',
        field: 'Buyer',
      },
      {
        headerName:'採購組織',
        field: 'Org',
      },
      {
        headerName:'文件日期',
        field: 'DocDate',
        valueFormatter:dateFormatter
      },
      {
        headerName:'廠商接收日期',
        field: 'ReplyDate',
        valueFormatter:dateFormatter
      },
      {
        headerName:'拋轉日期',
        field: 'CreateDate',
        valueFormatter:dateFormatter
      },
      { headerName: '操作', field: 'fieldName',
      cellRenderer : function(params){
        if(params.data.Status==10)
        {
          var eDiv = document.createElement('div');
          eDiv.innerHTML = '<span class="my-css-class"><button nz-button nzType="primary" class="btn-simple" style="height:39px">確認</button></span>';
          var eButton = eDiv.querySelectorAll('.btn-simple')[0];

          eButton.addEventListener('click', function() {
            _srmPoService.UpdateStatus(params.data.PoId).subscribe(result=>{
              alert('採購單號:'+params.data.PoNum+'已接收');
              params.data.Status=11;
              params.data.ReplyDate=new Date();
              params.api.refreshCells();
            });


          });

          return eDiv;
          }
        }
          //return '<button nz-button nzType="primary" (click)="alert(params.PoId)">確認</button>'

      }
      // {
      //   headerName:'操作',
      //   field: 'total',
      //   minWidth: 175,
      //   cellRenderer: 'totalValueRenderer',
      // },
    //   { field: 'calls' },
    //   {
    //     field: 'minutes',
    //     valueFormatter: "x.toLocaleString() + 'm'",
    //   },
    //   {
    //     field: 'date',
    //     editable: true,
    //     // valueFormatter: this.expiryDateFormatter,
    // cellEditorFramework: AgGridDatePickerComponent
    //   },
    ];
    this.defaultColDef = { flex: 1 };
    this.components = { datePicker: getDatePicker()};
    this.detailCellRendererParams = {
      detailGridOptions: {
        columnDefs: [
          {
            headerName:'採購單明細識別碼',
            field: 'PoLId',
          },
          {
            headerName:'採購單識別碼',
            field: 'PoId',
          },
          {
            headerName:'料號識別碼',
            field: 'MatnrId',
          },
          {
            headerName:'物料內文',
            field: 'Description',
          },
          {
            headerName:'數量',
            field: 'Qty',
          },
          {
            headerName:'單價',
            field: 'Price',
          },
          {
            headerName:'交貨日期',
            field: 'DeliveryDate',
            valueFormatter:dateFormatter
          },
          {
            headerName:'廠商交貨日期',
            field: 'ReplyDeliveryDate',
            editable: function(params){console.info(params.node.parent);return true},
            valueFormatter:dateFormatter,
            // valueFormatter: this.expiryDateFormatter,
            cellEditorFramework: AgGridDatePickerComponent
          },
          {
            headerName:'交貨地點',
            field: 'DeliveryPlace',
          },
          {
            headerName:'關鍵零組件/首件',
            field: 'CriticalPart',
          },
          {
            headerName:'檢驗時間',
            field: 'InspectionTime',
          },

        ],
        defaultColDef: { flex: 1 },
      },

      getDetailRowData: function (params) {
        console.info(params);
        params.successCallback(params.data.SrmPoLs);

      },
    };
  }

  ngOnInit(): void {
    this.searchForm = this._formBuilder.group({
      PO_NUM: [null],
      STATUS: [1],
      BUYER:[null]
    });
  }
  expiryDateFormatter(params) {
    if (params.value) {
      return `${params.value.date.month - 1}/${params.value.date.year}`;
     }
  }
  onFirstDataRendered(params) {
    // setTimeout(function () {
    //   params.api.getDisplayedRowAtIndex(1).setExpanded(true);
    // }, 0);
  }

  onGridReady(params) {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;
    this.getPoList(null);
    // this._srmPoService.GetPo(null)
    //   .subscribe((data) => {
    //     this.rowData = data;
    //   });
  }
  submitSearch() {
    this.refresh();
  }
  pageChange() {
    this.refresh();
  }
  refresh() {
    var query = {
      poNum: this.searchForm.value["PO_NUM"] == null ? "" : this.searchForm.value["PO_NUM"],
      status: this.searchForm.value["STATUS"] == null ? "0" : this.searchForm.value["STATUS"],
      buyer: this.searchForm.value["BUYER"] == null ? "" : this.searchForm.value["BUYER"],
      page: this.page,
      size: this.size
    }
    this.getPoList(query);
  }
  getPoList(query){
    if(query==null)
    {
      query = {
        poNum: "",
        status: "0",
        buyer: "",
        page: this.page,
        size: this.size
      }
    }
    this._srmPoService.GetPo(query)
      .subscribe((result) => {
        this.rowData = result;
      });
  }
}
function dateFormatter(data) {
  if(data.value==null) return "";
  var date=new Date(data.value);
  return `${date.getFullYear()}-${date.getMonth()+1}-${date.getDate()}`;
}
function getDatePicker() {
  function Datepicker() {}
  Datepicker.prototype.init = function (params) {
    this.eInput = document.createElement('input');
    this.eInput.value = params.value;
    this.eInput.classList.add('ag-input');
    this.eInput.style.height = '100%';
    $(this.eInput).datepicker({ dateFormat: 'dd/mm/yy' });
  };
  Datepicker.prototype.getGui = function () {
    return this.eInput;
  };
  Datepicker.prototype.afterGuiAttached = function () {
    this.eInput.focus();
    this.eInput.select();
  };
  Datepicker.prototype.getValue = function () {
    return this.eInput.value;
  };
  Datepicker.prototype.destroy = function () {};
  Datepicker.prototype.isPopup = function () {
    return false;
  };
  return Datepicker;
}
