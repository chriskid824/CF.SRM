import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
@Component({
  selector: 'app-delivery-receive',
  templateUrl: './delivery-receive.component.html',
  styleUrls: ['./delivery-receive.component.less']
})
export class DeliveryReceiveComponent implements OnInit {
  searchForm: FormGroup = new FormGroup({});
  rowData;
  constructor() { }

  ngOnInit(): void {
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
    //this.getDelyveryList(query);
  }
}
