CREATE DATABASE MediPlat
GO
USE MediPlat
GO

CREATE TABLE "Patient" (
  "ID" UNIQUEIDENTIFIER PRIMARY KEY,
  "UserName" NVARCHAR(255),
  "Email" NVARCHAR(255),
  "Password" NVARCHAR(255),
  "Balance" DECIMAL(12,2) DEFAULT 0,
  "Status" NVARCHAR(50)
);

CREATE TABLE "Profile"(
"ID" UNIQUEIDENTIFIER PRIMARY KEY,
"PatientID" UNIQUEIDENTIFIER,
"FullName" NVARCHAR(255),
"Sex" NVARCHAR(50),
"DOB" DATETIME,
"Address" NVARCHAR(MAX),
"JoinDate" DATETIME DEFAULT GETDATE(),
"PhoneNumber" NVARCHAR(20),
);

CREATE TABLE "Specialty" (
  "ID" UNIQUEIDENTIFIER PRIMARY KEY,
  "Name" NVARCHAR(255),
  "Description" NVARCHAR(MAX),
);

CREATE TABLE "Experience" (
  "ID" UNIQUEIDENTIFIER PRIMARY KEY,
  "SpecialtyID" UNIQUEIDENTIFIER,
  "Title" NVARCHAR(255),
  "Description" NVARCHAR(MAX),
  "Certificate" NVARCHAR(MAX),
  "DoctorID" UNIQUEIDENTIFIER,
  "Status" NVARCHAR(20) CHECK ("Status" IN (N'Active', N'Suspended'))
);

CREATE TABLE "Doctor" (
  "ID" UNIQUEIDENTIFIER PRIMARY KEY,
  "UserName" NVARCHAR(255),
  "FullName" NVARCHAR(255),
  "Email" NVARCHAR(255),
  "Password" NVARCHAR(255),
  "AvatarUrl" NVARCHAR(MAX),
  "Balance" DECIMAL(12,2) DEFAULT 0,
  "FeePerHour" DECIMAL(12,2),
  "Degree" NVARCHAR(255),
  "AcademicTitle" NVARCHAR(255),
  "JoinDate" DATETIME DEFAULT GETDATE(),
  "PhoneNumber" NVARCHAR(50),
  "Status" NVARCHAR(50),
  "AverageRating" DECIMAL(2,1) CHECK (AverageRating BETWEEN 1.0 AND 5.0)

);

CREATE TABLE "Subscription" (
  "ID" UNIQUEIDENTIFIER PRIMARY KEY,
  "Name" NVARCHAR(255),
  "Price" DECIMAL(18,2),
  "EnableSlot" TINYINT,
  "Description" NVARCHAR(MAX),
  "CreatedDate" DATETIME DEFAULT GETDATE(),
  "UpdateDate" DATETIME
);

CREATE TABLE "DoctorSubscription" (
  "ID" UNIQUEIDENTIFIER PRIMARY KEY,
  "SubscriptionID" UNIQUEIDENTIFIER,
  "EnableSlot" SMALLINT,
  "DoctorID" UNIQUEIDENTIFIER,
  "StartDate" DATETIME DEFAULT GETDATE(),
  "EndDate" DATETIME,
  "UpdateDate" DATETIME,
  "Status" NVARCHAR(20) CHECK ("Status" IN (N'Active', N'Inactive'))
);

CREATE TABLE "Services" (
  "ID" UNIQUEIDENTIFIER PRIMARY KEY,
  "SpecialtyID" UNIQUEIDENTIFIER,
  "Title" NVARCHAR(255),
  "Description" NVARCHAR(MAX)
);

CREATE TABLE "Slot" (
  "ID" UNIQUEIDENTIFIER PRIMARY KEY,
  "DoctorID" UNIQUEIDENTIFIER,
  "ServiceID" UNIQUEIDENTIFIER,
  "Title" NVARCHAR(255),
  "Description" NVARCHAR(MAX),
  "StartTime" DATETIME,
  "EndTime" DATETIME,
  "Date" DATETIME,
  "SessionFee" DECIMAL(12,2),
  "Status" NVARCHAR(50)
);

CREATE TABLE "Transaction" (
  "ID" UNIQUEIDENTIFIER PRIMARY KEY,
  "PatientID" UNIQUEIDENTIFIER,
  "DoctorID" UNIQUEIDENTIFIER,
  "SubID" UNIQUEIDENTIFIER,
  "AppointmentSlotID" UNIQUEIDENTIFIER,
  "TransactionInfo" NVARCHAR(MAX),
  "Amount" DECIMAL(18,2),
  "CreatedDate" DATETIME DEFAULT GETDATE(),
  "Status" NVARCHAR(50) CHECK ("Status" IN (N'Pending', N'Completed', N'Failed')) DEFAULT 'Pending' 
);

CREATE TABLE "Review" (
  "ID" UNIQUEIDENTIFIER PRIMARY KEY,
  "SlotID" UNIQUEIDENTIFIER UNIQUE,
  "Rating" TINYINT CHECK (rating BETWEEN 1 AND 5),
  "Message" NVARCHAR(MAX),
  "CreatedDate" DATETIME DEFAULT GETDATE(),
);

CREATE TABLE "AppointmentSlot" (
  "ID" UNIQUEIDENTIFIER PRIMARY KEY,
  "SlotID" UNIQUEIDENTIFIER,
  "ProfileID" UNIQUEIDENTIFIER,
  "Status" NVARCHAR(50) NOT NULL CHECK ("Status" IN (N'Pending', N'Confirmed', N'Completed', N'Cancelled', N'Pending Confirmation')),
  "CreatedDate" DATETIME DEFAULT GETDATE(),
  "Notes" NVARCHAR(MAX)
);

CREATE TABLE "Medicine" (
  "ID" UNIQUEIDENTIFIER PRIMARY KEY,
  "Name" NVARCHAR(255) NOT NULL,
  "DosageForm" NVARCHAR(100), -- Dạng thuốc (Viên nén, Siro, Tiêm,...)
  "Strength" NVARCHAR(50) NOT NULL, -- Hàm lượng (500mg, 200mg,...)
  "SideEffects" NVARCHAR(MAX) -- Tác dụng phụ
);

CREATE TABLE "AppointmentSlotMedicine" (
  "AppointmentSlotMedicineID" UNIQUEIDENTIFIER PRIMARY KEY,
  "AppointmentSlotID" UNIQUEIDENTIFIER,
  "MedicineID" UNIQUEIDENTIFIER,
  "Dosage" NVARCHAR(255) NOT NULL, -- Liều lượng kê đơn (vd: "2 viên/ngày")
  "Instructions" NVARCHAR(MAX), -- Hướng dẫn sử dụng(uống sau khi ăn, uống trước khi ăn)
  "Quantity" SMALLINT NOT NULL, 
  );

ALTER TABLE "Profile" ADD FOREIGN KEY ("PatientID") REFERENCES "Patient" ("ID");

ALTER TABLE "Experience" ADD FOREIGN KEY ("SpecialtyID") REFERENCES "Specialty" ("ID");

ALTER TABLE "Experience" ADD FOREIGN KEY ("DoctorID") REFERENCES "Doctor" ("ID");

ALTER TABLE "DoctorSubscription" ADD FOREIGN KEY ("SubscriptionID") REFERENCES "Subscription" ("ID");

ALTER TABLE "DoctorSubscription" ADD FOREIGN KEY ("DoctorID") REFERENCES "Doctor" ("ID");

ALTER TABLE "Services" ADD FOREIGN KEY ("SpecialtyID") REFERENCES "Specialty" ("ID");

ALTER TABLE "Slot" ADD FOREIGN KEY ("DoctorID") REFERENCES "Doctor" ("ID");

ALTER TABLE "Slot" ADD FOREIGN KEY ("ServiceID") REFERENCES "Services" ("ID");

ALTER TABLE "AppointmentSlot" ADD FOREIGN KEY ("SlotID") REFERENCES "Slot" ("ID");

ALTER TABLE "AppointmentSlot" ADD FOREIGN KEY ("ProfileID") REFERENCES "Profile" ("ID");

ALTER TABLE "Transaction" ADD FOREIGN KEY ("AppointmentSlotID") REFERENCES "AppointmentSlot" ("ID");

ALTER TABLE "Transaction" ADD FOREIGN KEY ("SubID") REFERENCES "Subscription" ("ID");

ALTER TABLE "Transaction" ADD FOREIGN KEY ("PatientID") REFERENCES "Patient" ("ID");

ALTER TABLE "Transaction" ADD FOREIGN KEY ("DoctorID") REFERENCES "Doctor" ("ID");

ALTER TABLE "Review" ADD FOREIGN KEY ("SlotID") REFERENCES "Slot" ("ID");

ALTER TABLE "AppointmentSlotMedicine" ADD FOREIGN KEY ("AppointmentSlotID") REFERENCES "AppointmentSlot" ("ID");

ALTER TABLE "AppointmentSlotMedicine" ADD FOREIGN KEY ("MedicineID") REFERENCES "Medicine" ("ID");

INSERT INTO Doctor(ID, UserName, FullName, Email, "Password", AvatarUrl, Balance, FeePerHour, Degree, AcademicTitle, JoinDate, PhoneNumber, Status)
VALUES
	(NEWID(), N'tuanntase140515', N'Nguyễn Thanh Anh Tuấn', 'tuanntase140515@gmail.com', N'12345', CONCAT('https://api.dicebear.com/7.x/pixel-art/png?seed=', CAST(NEWID() AS NVARCHAR(50))), 10000.00, 10000.00, N'Bác sĩ Đa Khoa', N'Giáo Sư', GETDATE(), '0705543619', 'Active');
INSERT INTO Patient(ID, UserName, FullName, Email, "Password", PhoneNumber, Balance, JoinDate, Sex, "Address", "Status")
VALUES
	(NewID(), N'TuanNTA', N'Tuan Nguyen', N'tuannguyen123@gmail.com', N'12345', '0705543618', 10000, GETDATE(), 'Male', N'123 Lý Trự Trọng', N'Active');
INSERT INTO Subscription(ID, "Name", Price, EnableSlot, "Description", CreatedDate, UpdateDate)
VALUES
	(NEWID(), N'Gói cơ bản', 1500000, 50, N'Gói cơ bản có 50 slots khám bệnh', GETDATE(), null),
	(NEWID(), N'Gói nâng cấp 1', 2900000, 100, N'Gói nâng cấp 1 có 100 slots khám bệnh', GETDATE(), null),
	(NEWID(), N'Gói nâng cấp 2', 4050000, 150, N'Gói nâng cấp 2 có 150 slots khám bệnh', GETDATE(), null),
	(NEWID(), N'Gói nâng cấp 3', 4800000, 200, N'Gói nâng cấp 3 có 200 slots khám bệnh', GETDATE(), null),
	(NEWID(), N'Gói cao cấp', 1500000, 250, N'Gói cao cấp có 250 slots khám bệnh', GETDATE(), null)

INSERT INTO Specialty (ID, Name, Description)
VALUES 
    (NEWID(), N'Nội tổng quát', N'Khám và điều trị các bệnh nội khoa tổng quát.'),
    (NEWID(), N'Tim mạch', N'Chuyên khám và điều trị bệnh về tim mạch.'),
    (NEWID(), N'Hô hấp', N'Chuyên khoa điều trị các bệnh về đường hô hấp.'),
    (NEWID(), N'Tiêu hóa', N'Chuyên khoa nội soi và điều trị bệnh tiêu hóa.'),
    (NEWID(), N'Nội tiết - Đái tháo đường', N'Điều trị các bệnh về nội tiết và tiểu đường.'),
    (NEWID(), N'Thận - Tiết niệu', N'Chẩn đoán và điều trị các bệnh về thận, đường tiết niệu.'),
    (NEWID(), N'Thần kinh', N'Chuyên khoa về hệ thần kinh, động kinh, Parkinson.'),
    (NEWID(), N'Cơ xương khớp', N'Chuyên điều trị viêm khớp, thoái hóa cột sống.'),
    (NEWID(), N'Huyết học - Truyền máu', N'Chuyên điều trị bệnh máu và truyền máu.'),
    (NEWID(), N'Nhi khoa', N'Khám và điều trị bệnh cho trẻ em.'),
    (NEWID(), N'Da liễu', N'Điều trị bệnh ngoài da, dị ứng, mụn trứng cá.'),
    (NEWID(), N'Tâm thần', N'Tâm lý, rối loạn lo âu, trầm cảm.'),
    (NEWID(), N'Dị ứng - Miễn dịch', N'Chuyên điều trị dị ứng, rối loạn miễn dịch.'),
    
    -- Chuyên khoa ngoại
    (NEWID(), N'Ngoại tổng quát', N'Chuyên phẫu thuật các bệnh lý ngoại khoa.'),
    (NEWID(), N'Ngoại thần kinh', N'Phẫu thuật não, cột sống.'),
    (NEWID(), N'Ngoại tim mạch', N'Phẫu thuật tim, mạch máu.'),
    (NEWID(), N'Ngoại tiêu hóa', N'Phẫu thuật dạ dày, đại tràng.'),
    (NEWID(), N'Ngoại tiết niệu', N'Phẫu thuật sỏi thận, bàng quang.'),
    (NEWID(), N'Chấn thương chỉnh hình', N'Điều trị gãy xương, thoát vị đĩa đệm.'),
    
    -- Sản phụ khoa
    (NEWID(), N'Sản - Phụ khoa', N'Chăm sóc sức khỏe sinh sản, sinh đẻ.'),
    (NEWID(), N'Hiếm muộn - Vô sinh', N'Điều trị hiếm muộn, hỗ trợ sinh sản.'),
    (NEWID(), N'Nam khoa', N'Khám và điều trị bệnh nam khoa.'),
    
    -- Tai - Mũi - Họng
    (NEWID(), N'Tai - Mũi - Họng', N'Khám và điều trị bệnh về tai, mũi, họng.'),
    
    -- Mắt (Nhãn khoa)
    (NEWID(), N'Nhãn khoa', N'Điều trị các bệnh về mắt, cận thị, viễn thị.'),
	(NEWID(), N'Nhãn khoa', N'Bệnh giác mạc.'),
	(NEWID(), N'Nhãn khoa', N'Điều trị các bệnh về mắt, cận thị, viễn thị.'),
    
    -- Răng - Hàm - Mặt
    (NEWID(), N'Nha khoa', N'Điều trị răng miệng, chỉnh nha.'),
	(NEWID(), N'Nha khoa', N'Chỉnh nha (Niềng răng).'),
	(NEWID(), N'Nha khoa', N'Nha khoa thẩm mỹ (Bọc răng sứ, tẩy trắng răng,...).'),
	(NEWID(), N'Nha khoa', N'Cấy ghép Implant.'),
	(NEWID(), N'Nha khoa', N'Phẫu thuật hàm mặt.'),
    
    -- Ung bướu
    (NEWID(), N'Ung bướu', N'Ung thư nội khoa.'),
    (NEWID(), N'Ung bướu', N'Ung thư ngoại khoa.'),
	(NEWID(), N'Ung bướu', N'Điều trị và xạ trị ung thư.'),
    -- Y học cổ truyền
    (NEWID(), N'Y học cổ truyền', N'Châm cứu, bấm huyệt, thuốc Đông y.'),
    
    -- Phục hồi chức năng
    (NEWID(), N'Phục hồi chức năng', N'Điều trị vật lý trị liệu.'),
    
    -- Bệnh truyền nhiễm
    (NEWID(), N'Bệnh truyền nhiễm', N'Điều trị các bệnh truyền nhiễm, sốt xuất huyết.'),
    
    -- Cấp cứu - Hồi sức
    (NEWID(), N'Cấp cứu - Hồi sức', N'Cấp cứu bệnh nhân nguy kịch.');

	INSERT INTO Experience (ID, SpecialtyID, Title, Description, Certificate, DoctorID, Status)
VALUES 
    (NEWID(), '7B70F254-DC7F-48F2-804D-30EE3C56E989', N'Chuyên gia Da liễu', N'Chuyên điều trị các bệnh ngoài da, dị ứng, mụn trứng cá.', N'Chứng nhận Da liễu cấp bởi Bộ Y Tế', 'B9593D05-DB30-4041-819C-46D3A93E4E56', N'Active'),

    (NEWID(), 'AC24AE47-3247-4307-AB32-31751F7A2357', N'Chuyên gia Thận - Tiết niệu', N'Điều trị bệnh lý về thận, sỏi thận, viêm đường tiết niệu.', N'Chứng nhận chuyên khoa Thận - Tiết niệu', 'B9593D05-DB30-4041-819C-46D3A93E4E56', N'Active'),

    (NEWID(), '14AAD688-F03F-42F9-AC35-3EEE82AFBB68', N'Chuyên gia Phục hồi chức năng', N'Thực hiện điều trị vật lý trị liệu cho bệnh nhân sau tai nạn, phẫu thuật.', N'Chứng nhận Phục hồi chức năng từ Hiệp hội Y học phục hồi Việt Nam', 'B9593D05-DB30-4041-819C-46D3A93E4E56', N'Active');
