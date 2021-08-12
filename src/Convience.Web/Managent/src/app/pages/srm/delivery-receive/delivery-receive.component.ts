import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { SrmDeliveryService } from '../../../business/srm/srm-delivery.service';
import { ActivatedRoute } from '@angular/router';
@Component({
  selector: 'app-delivery-receive',
  templateUrl: './delivery-receive.component.html',
  styleUrls: ['./delivery-receive.component.less']
})
export class DeliveryReceiveComponent implements OnInit {
  searchForm: FormGroup = new FormGroup({});
  rowData;
  gridApi;
  gridColumnApi;
  deliveryLId;
  deliveryNum;
  constructor(private route: ActivatedRoute,private _formBuilder: FormBuilder,private _srmDeliveryService: SrmDeliveryService) { }

  ngOnInit(): void {
    const routeParams = this.route.snapshot.paramMap;
    this.deliveryLId = routeParams.get('deliveryLId');
    this.deliveryNum = routeParams.get('deliveryNum');
    this.searchForm = this._formBuilder.group({
      DELIVERY_NUM: this.deliveryNum,
      DELIVERY_L_ID: this.deliveryLId,
    });
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
      //deliveryLId: this.searchForm.value["DELIVERY_L_ID"] == null ? "0" : this.searchForm.value["DELIVERY_L_ID"],
      deliveryLId:this.deliveryLId,
    }
    this.getDelyveryList(query);
  }
  getDelyveryList(query){
    if(query==null)
    {
      query = {
        deliveryNum:this.deliveryNum,
        deliveryLId:this.deliveryLId,
      }
    }
    this._srmDeliveryService.GetDeliveryL(query)
      .subscribe((result) => {
        console.info(result);
        this.rowData = result[0].SrmDeliveryLs;
      });
  }
}
