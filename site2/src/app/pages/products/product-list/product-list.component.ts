import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ItemService } from '../../../services/item.service';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './product-list.component.html',
  styles: []
})
export class ProductListComponent implements OnInit {
  items: any[] = [];
  isLoading = true;
  error = '';

  constructor(private itemService: ItemService) {}

  ngOnInit(): void {
    this.loadItems();
  }

  loadItems(): void {
    this.isLoading = true;
    this.itemService.getItems().subscribe({
      next: (data) => {
        this.items = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading items:', err);
        this.error = 'Error al cargar los productos';
        this.isLoading = false;
      }
    });
  }
}
