import { Component, OnInit, Inject } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";
import { DomSanitizer, SafeHtml } from "@angular/platform-browser";

@Component({
  selector: 'app-delivery-modal',
  templateUrl: './delivery-modal.component.html',
  styleUrls: ['./delivery-modal.component.less']
})
export class DeliveryModalComponent implements OnInit {
  columnDefs: any;
  defaultColDef;
  rowData: [];
  rowGroupPanelShow;
  sideBar;
  Title;
  deliverydata;
  iscf:boolean=false;
  constructor(@Inject(MAT_DIALOG_DATA) public data: any) {
    this.rowGroupPanelShow='always';
    // var defs=[];
    // Object.keys(data.data[0]).forEach(function (key) {
    //   defs.push({ field: key });
    // });
    this.columnDefs = [
      {
        headerName:'項次',
        field: 'DeliveryLId',
        suppressSizeToFit: false,
      },
      {
        headerName:'物料',
        field: 'Mantr',
        suppressSizeToFit: false,
      },
      {
        headerName:'採購單號',
        field: 'PoNum',
        suppressSizeToFit: false,
      },
      {
        headerName:'採購項次',
        field: 'PoLId',
        suppressSizeToFit: false,
      },
      {
        headerName:'數量',
        field: 'CreateDate',
        suppressSizeToFit: false,
      },
      {
        headerName:'單位',
        field: 'CreateBy',
        suppressSizeToFit: false,
      },
      {
        headerName:'物料說明',
        field: 'CreateBy',
        suppressSizeToFit: false,
      },
      {
        headerName:'工單號碼',
        field: 'CreateBy',
        suppressSizeToFit: false,
      },
      {
        headerName:'工單項次',
        field: 'CreateBy',
        suppressSizeToFit: false,
      },
    ];
    this.defaultColDef = {
      editable: true,
      filter: "agTextColumnFilter",
      allowedAggFuncs: ['sum', 'min', 'max'],
      enableValue: true,
      enableRowGroup: true,
      enablePivot: true,
      // floatingFilter: true,
      resizable: true,
      cellStyle: function(params) {
        if (typeof params.value === 'number') {
            return {'text-align': 'right'};
        } else {
            return null;
        }
    }
    };
    this.deliverydata=this.data.data;
    this.rowData = this.data.data.SrmDeliveryLs;
    if(this.data.data.SrmDeliveryLs.length>0&&this.data.data.SrmDeliveryLs[0].Org!='3100')
    {
      this.iscf=true;
    }
    console.info(this.deliverydata);
    this.Title=this.data.paramname+" - "+this.data.columnname+" - "+this.data.valuetype;
    this.sideBar = {
      toolPanels: [
        {
          id: 'columns',
          labelDefault: 'Columns',
          labelKey: 'columns',
          iconKey: 'columns',
          toolPanel: 'agColumnsToolPanel',
        },
        {
          id: 'filters',
          labelDefault: 'Filters',
          labelKey: 'filters',
          iconKey: 'filter',
          toolPanel: 'agFiltersToolPanel',
        },
      ],
      defaultToolPanel: 'columns',
      hiddenByDefault: false,
    };
    //console.warn(this.data.keys);
  }
  onGridReady(params) {
    //params.api.closeToolPanel();
    params.api.sizeColumnsToFit();
    var allColumnIds = [];
    params.columnApi.getAllColumns().forEach(function (column) {
      allColumnIds.push(column.colId);
    });
    //params.columnApi.autoSizeColumns(allColumnIds, true);
  }
  ngOnInit(): void {}
print(){
  printElement(document.getElementById("printThis"));
  //window.print();
}
}
function printElement(elem) {
  var domClone = elem.cloneNode(true);
  var domClone2 = elem.cloneNode(true);
  var $printSection = document.getElementById("printSection");

  if (!$printSection) {
      var $printSection2 = document.createElement("div");
      $printSection2.id = "printSection";
      document.body.appendChild($printSection2);
      document.body.appendChild($printSection2);
      // $printSection2.innerHTML = "";
      // $printSection2.appendChild(domClone);
      // window.print();
      // return;
  }
  $printSection = document.getElementById("printSection");
  $printSection.innerHTML = "";
  $printSection.appendChild(domClone);
  $printSection.appendChild(domClone2);
  console.log($printSection);

  window.print();
}
