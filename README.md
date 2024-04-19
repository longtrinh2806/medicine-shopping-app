**Medicine Shopping App**

Ý tưởng chính của việc tạo dự án này là vận hành một hệ thống phân tán với công nghệ và kiến ​​trúc mới nhất như Microservices, DDD, MongoDB, MSSQL, Redis, RabbitMq, Masstransit trong .Net 8.

**The Goals of This Project**
❇️ Sử dụng kiến trúc microservices để thiết kế kiến trúc cho dự án.
❇️ Sử dụng Domain Driven Design (DDD) để thực hiện những business logic trong dự án.
❇️ Sử dụng Rabbitmq và Masstransit cho việc giao tiếp giữa các services.
❇️ Sử dụng MSSQL và MongoDB làm hệ quản trị cơ sở dữ liệu.
❇️ Sử dụng các kĩ thuật tối ưu CSDL (indexes, database replica)
❇️ Sử dụng Docker-Compose để đóng gói ứng dụng.
❇️ Sử dụng JWT cho việc xác thực và phân quyền.
❇️ Sử dụng Ocelot làm microservices gateway.

**Technologies - Libraries**
✔️ .NET 8 - .NET Framework và .NET Core, bao gồm ASP.NET and ASP.NET Core
✔️ Spring Boot 3.2.5 - Spring Framework
✔️ Mapster - Convention-based object-object mapper in .NET.
✔️ Masstransit - Distributed Application Framework for .NET.
✔️ Swagger & Swagger UI - Swagger tools cho việc tự động generate API Document.
✔️ Ocelot - API Gateway
✔️ MongoDB.Driver - .NET Driver for MongoDB.
✔️ Microsoft.EntityFrameworkCore.SqlServer - .NET Driver for MSSQL.

**Tổng quan về hệ thống**
- Authentication Service: chịu trách nhiệm cho việc tạo mới, xác thực và phân quyền người dùng. Service này sử dụng Spring Boot, MSSQL và Jwt cho việc authentication và authorization.
- Inventory Service: chịu trách nhiệm cho việc lưu trữ quản lí sản phẩm. Service này sử dụng .NET 8 và MSSQL.
- Customer Service: chịu trách nhiệm cho việc lưu trữ thông tin khách hàng cũng như điểm thưởng tích lũy.
- Order Service: chịu trách nhiệm cho việc lưu trữ và quản lí các business liên quan tới việc đặt hàng.
- Redis Caching: chịu trách nhiệm cho việc caching data.

**Dưới đây là Sơ đồ tổng quan của hệ thống:**

![image](https://github.com/longtrinh2806/medicine-shopping-app/assets/136159911/eef29b9a-e0ae-45b3-af75-9499bb86c65f)

**Cách chạy ứng dụng thông qua docker**
1. Clone github repo
2. Truy cập vào thư mục medicine-shopping-app, nơi chứa file docker compose. Sau đó mở cmd và chạy lệnh sau: docker compose up (hoặc docker compose up -d). Đảm bảo máy đã cài đặt Docker Desktop.
![image](https://github.com/longtrinh2806/medicine-shopping-app/assets/136159911/2f9447c7-41be-4db9-8ae9-7ca23b17b2e1)
3. Sau khi chạy thành công, truy cập đường dẫn sau để xem tài liệu swagger: http://localhost:{port}/swagger/index.html
Thay {port} thành port cần truy cập. Đây là danh sách các port hiện hành:
    5001: Product Service
    5002: Customer Service
    5003: Order Service
Đối với Authentication Service, chưa có tài liệu swagger. Microservices này chạy ở port 5004 và có các api và chức năng như sau:
![image](https://github.com/longtrinh2806/medicine-shopping-app/assets/136159911/6b5997e0-45f8-4bdf-99b8-a1440f9e949d)
![image](https://github.com/longtrinh2806/medicine-shopping-app/assets/136159911/062d93f8-d9f7-4b27-9134-28db225bfd7b)
![image](https://github.com/longtrinh2806/medicine-shopping-app/assets/136159911/40ad8fa5-e512-4dad-8194-d771561053d2)

Nếu khi chạy ứng dụng mà bị lỗi ở service authentication, vui lòng cung cấp ip hiện tại. Bởi vì app đang sử dụng database free do azure cung cấp nên bị hạn chế về chức năng và tính sẵn sàng.
