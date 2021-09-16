import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, FormControl} from '@angular/forms';

@Component({
  selector: 'app-supplier-c',
  templateUrl: './supplier-c.component.html',
  styleUrls: ['./supplier-c.component.less']
})
export class SupplierCComponent implements OnInit {
  formDetail: FormGroup = new FormGroup({});

  constructor() { }

  ngOnInit(): void { }

}
