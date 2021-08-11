import { Component, OnInit,ViewEncapsulation } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NzDatePickerComponent} from 'ng-zorro-antd/date-picker';
import datepickerFactory from 'jquery-datepicker';
import datepickerJAFactory from 'jquery-datepicker/i18n/jquery.ui.datepicker-en-GB';
import { AgGridDatePickerComponent} from '../po/AGGridDatePickerCompponent';
import { SrmPoService } from '../../../business/srm/srm-po.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
@Component({
  selector: 'app-delyvery-l',
  encapsulation: ViewEncapsulation.None,
  templateUrl: './delyvery-l.component.html',
  styleUrls: ['./delyvery-l.component.less']
})
export class DelyveryLComponent implements OnInit {
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
        headerName:'交貨單號',
        field: 'DeliveryNum',
        cellRenderer: 'agGroupCellRenderer',
      },
      {
        headerName:'交貨單識別碼',
        field: 'DeliveryId',
        hide:true
      },
      {
        headerName:'狀態',
        field: 'Status',
        cellClassRules: {
          'rag-green': 'x == 16',
          'rag-red': 'x == 15',
        },
        valueFormatter:'switch(value){case 15 : return "待出貨"; case 16 : return "已出貨"; default : return "未知";}'
      },
      {
        headerName:'交貨日期',
        field: 'DeliveryDate',
        valueFormatter:dateFormatter
      },
      {
        headerName:'建立日期',
        field: 'CreateDate',
        valueFormatter:dateFormatter
      },
      {
        headerName:'建立人員',
        field: 'CreateBy',
      },
       { headerName: '操作', field: 'fieldName',
       cellRenderer : function(params){
         if(params.data.Status==15)
         {
           var eDiv = document.createElement('div');
           eDiv.innerHTML = '<span class="my-css-class"><button nz-button nzType="primary" class="btn-simple" style="height:39px">列印出貨單</button></span>';
           var eButton = eDiv.querySelectorAll('.btn-simple')[0];

          //  eButton.addEventListener('click', function() {
          //    _srmPoService.UpdateStatus(params.data.PoId).subscribe(result=>{
          //      alert('採購單號:'+params.data.DeliveryNum+'已交貨');
          //      params.data.Status=11;
          //      params.data.ReplyDate=new Date();
          //     params.api.refreshCells();
          //   });
          //  });
           return eDiv;
           }
         }
       }
    ];
    this.defaultColDef = { flex: 1 };
    this.components = { datePicker: getDatePicker()};
    this.detailCellRendererParams = {
      detailGridOptions: {
        columnDefs: [
          {
            headerName:'交貨單明細識別碼',
            field: 'DeliveryLId',
          },
          {
            headerName:'採購單識別碼',
            field: 'PoNum',
          },
          {
            headerName:'採購明細識別碼',
            field: 'PoLId',
          },
          {
            headerName:'交貨數量',
            field: 'DeliveryQty',
          },
          {
            headerName:'已品檢數量',
            field: 'QmQty',
          },

        ],
        defaultColDef: { flex: 1 },
      },

      getDetailRowData: function (params) {
        console.info(params);
        params.successCallback(params.data.SrmDeliveryLs);

      },
    };
  }

  ngOnInit(): void {
    this.searchForm = this._formBuilder.group({
      DELIVERY_NUM: [null],
      STATUS: [1],
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
    this.getDelyveryList(null);
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
      deliveryNum: this.searchForm.value["DELIVERY_NUM"] == null ? "" : this.searchForm.value["DELIVERY_NUM"],
      status: this.searchForm.value["STATUS"] == null ? "0" : this.searchForm.value["STATUS"],
    }
    this.getDelyveryList(query);
  }
  getDelyveryList(query){
    if(query==null)
    {
      query = {
        deliveryNum: "",
        status: "0",
      }
    }
    this._srmPoService.GetDelivery(query)
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
  // Datepicker.prototype.init = function (params) {
  //   this.eInput = document.createElement('input');
  //   this.eInput.value = params.value;
  //   this.eInput.classList.add('ag-input');
  //   this.eInput.style.height = '100%';
  //   $(this.eInput).datepicker({ dateFormat: 'dd/mm/yy' });
  // };
  // Datepicker.prototype.getGui = function () {
  //   return this.eInput;
  // };
  // Datepicker.prototype.afterGuiAttached = function () {
  //   this.eInput.focus();
  //   this.eInput.select();
  // };
  // Datepicker.prototype.getValue = function () {
  //   return this.eInput.value;
  // };
  // Datepicker.prototype.destroy = function () {};
  // Datepicker.prototype.isPopup = function () {
  //   return false;
  // };
  // return Datepicker;
}
