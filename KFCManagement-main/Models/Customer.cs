using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KFCManagement.Models
{
    public partial class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Tên khách hàng không được để trống.")]
        [Display(Name = "Tên khách hàng")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = null!;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [Display(Name = "Số điện thoại")]
        public string? Phone { get; set; }

        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime? CreatedAt { get; set; }

        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "Danh sách đơn hàng")]
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        [Display(Name = "Danh sách đặt chỗ")]
        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
