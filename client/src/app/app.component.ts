import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent  implements OnInit
{
  title = 'e Commerce App';
  products: any;
  users: any;

  constructor(private http: HttpClient)
  {
    
  }

  ngOnInit(): void 
  {
    this.getProducts();
    this.getUsers();
  }

  getProducts()
  {
    this.http.get('https://localhost:5001/products/').subscribe( response => {
      this.products = response;
    }, error => {
      console.log(error);
    })
  }

  getUsers()
  {
    this.http.get('https://localhost:5001/users/').subscribe( response => {
      this.users = response;
    }, error => {
      console.log(error);
    })
  }
}
