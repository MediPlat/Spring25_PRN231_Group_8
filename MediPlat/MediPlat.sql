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
  "Status" NVARCHAR(50) NOT NULL CHECK ("Status" IN (N'Pending', N'Confirmed', N'Cancelled')),
  "CreatedDate" DATETIME DEFAULT GETDATE(),
  "Notes" NVARCHAR(MAX)
);

CREATE TABLE "Medicine" (
  "ID" UNIQUEIDENTIFIER PRIMARY KEY,
  "Name" NVARCHAR(255) NOT NULL,
  "DosageForm" NVARCHAR(100), -- Dạng thuốc (Viên nén, Siro, Tiêm,...)
  "Strength" NVARCHAR(50) NOT NULL, -- Hàm lượng (500mg, 200mg,...)
  "SideEffects" NVARCHAR(MAX), -- Tác dụng phụ
  "Status" NVARCHAR(50) NOT NULL DEFAULT 'Active'
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

INSERT INTO Patient(ID, UserName, Email, "Password", Balance, "Status")
VALUES
	(NewID(), N'TuanNTA', N'tuannguyen123@gmail.com', N'123', 10000, N'Active');

INSERT INTO "Profile"(ID, PatientID, FullName, Sex, DOB, Address, JoinDate, PhoneNumber)
VALUES
    (NEWID(), '6E5BD584-140D-4294-9165-44E7D3CCD27B', N'Nguyễn Văn An', N'Nam', '1990-05-15', N'123 Đường Lê Lợi, Hà Nội', GETDATE(), '0912345678'),
    (NEWID(), '6E5BD584-140D-4294-9165-44E7D3CCD27B', N'Trần Thị Bích', N'Nữ', '1995-08-22', N'456 Đường Trần Hưng Đạo, TP. Hồ Chí Minh', GETDATE(), '0987654321'),
    (NewID(), '6E5BD584-140D-4294-9165-44E7D3CCD27B', N'Lê Anh Nuôi', N'Male', '2000-04-24', N'123 Trần Phú', GetDate(), N'0903383891')

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

INSERT INTO "Medicine" (ID, Name, DosageForm, Strength, SideEffects, Status)
VALUES
    (NEWID(), N'Paracetamol', N'Viên nén', N'500mg', N'Buồn nôn, chóng mặt, phát ban', N'Active'),
    (NEWID(), N'Amoxicillin', N'Viên nang', N'500mg', N'Tiêu chảy, buồn nôn, dị ứng', N'Active'),
    (NEWID(), N'Ibuprofen', N'Viên nén', N'400mg', N'Đau dạ dày, nhức đầu, buồn ngủ', N'Active'),
    (NEWID(), N'Cetirizine', N'Viên nén', N'10mg', N'Buồn ngủ, khô miệng, đau đầu', N'Active'),
    (NEWID(), N'Metformin', N'Viên nén', N'850mg', N'Buồn nôn, tiêu chảy, đau bụng', N'Active'),
    (NEWID(), N'Omeprazole', N'Viên nang', N'20mg', N'Buồn nôn, tiêu chảy, táo bón', N'Active'),
    (NEWID(), N'Diclofenac', N'Viên nén', N'50mg', N'Buồn nôn, tiêu chảy, đau đầu', N'Active'),
    (NEWID(), N'Aspirin', N'Viên nén', N'81mg', N'Đau dạ dày, chảy máu, phát ban', N'Active'),
    (NEWID(), N'Atorvastatin', N'Viên nén', N'20mg', N'Đau cơ, tiêu chảy, mệt mỏi', N'Active'),
    (NEWID(), N'Losartan', N'Viên nén', N'50mg', N'Chóng mặt, đau đầu, mệt mỏi', N'Active');

INSERT INTO [MediPlat].[dbo].[Specialty] (ID, Name, Description)
VALUES
    (NEWID(), N'Chứng chỉ Nội khoa', N'Chứng chỉ chuyên sâu về chẩn đoán và điều trị bệnh nội khoa.'),
    (NEWID(), N'Chứng chỉ Ngoại khoa', N'Chứng chỉ chuyên về phẫu thuật và điều trị bệnh ngoại khoa.'),
    (NEWID(), N'Chứng chỉ Nhi khoa', N'Chứng chỉ chuyên chăm sóc và điều trị bệnh nhi.'),
    (NEWID(), N'Chứng chỉ Da liễu', N'Chứng chỉ chuyên khoa về chẩn đoán và điều trị bệnh da liễu.'),
    (NEWID(), N'Chứng chỉ Sản phụ khoa', N'Chứng chỉ chuyên về chăm sóc sức khỏe sinh sản và phụ nữ mang thai.'),
    (NEWID(), N'Chứng chỉ Tim mạch', N'Chứng chỉ chuyên khoa tim mạch về điều trị và quản lý bệnh tim.'),
    (NEWID(), N'Chứng chỉ Thần kinh', N'Chứng chỉ chuyên về chẩn đoán và điều trị bệnh thần kinh.'),
    (NEWID(), N'Chứng chỉ Chấn thương chỉnh hình', N'Chứng chỉ chuyên về điều trị chấn thương xương khớp và phục hồi chức năng.'),
    (NEWID(), N'Chứng chỉ Tai Mũi Họng', N'Chứng chỉ chuyên khoa về chẩn đoán và điều trị bệnh tai, mũi, họng.'),
    (NEWID(), N'Chứng chỉ Nhãn khoa', N'Chứng chỉ chuyên về chẩn đoán và điều trị bệnh về mắt.');

INSERT INTO "Services" (ID, SpecialtyID, Title, Description)
VALUES
    (NEWID(), 'ECB0BA3D-02D5-4525-8ED6-41C22B6E0D8B', N'Phẫu thuật chỉnh hình', N'Dịch vụ phẫu thuật và điều trị chấn thương chỉnh hình.'),
    (NEWID(), 'BE1C514B-FA69-465B-992B-6DD3029CC758', N'Tư vấn nội khoa', N'Tư vấn sức khỏe và điều trị các bệnh nội khoa.'),
    (NEWID(), '9B50DBE2-8F7A-4C4E-8F05-88332AD31E54', N'Tầm soát bệnh tim mạch', N'Kiểm tra, tư vấn và điều trị các bệnh về tim mạch.'),
    (NEWID(), '44090894-FA02-49F2-B7E4-C196FAC05026', N'Khám nhi tổng quát', N'Kiểm tra và chăm sóc sức khỏe tổng quát cho trẻ em.'),
    (NEWID(), '361F16D5-4683-4FA5-B9C3-C861485ED137', N'Phẫu thuật ngoại khoa', N'Điều trị các bệnh cần can thiệp phẫu thuật.'),
    (NEWID(), '8F37658C-05BD-4A27-830C-CB963D09E036', N'Kiểm tra thị lực', N'Kiểm tra mắt và tư vấn điều trị bệnh về mắt.'),
    (NEWID(), '449A99C4-2DBE-4C9E-9631-E0C69F461168', N'Tư vấn sức khỏe sinh sản', N'Kiểm tra và tư vấn sức khỏe sinh sản cho phụ nữ.'),
    (NEWID(), '384C4F3D-E897-4ADD-8EBD-E3B9425EFF84', N'Điều trị bệnh về tai mũi họng', N'Chẩn đoán và điều trị các bệnh liên quan đến tai mũi họng.'),
    (NEWID(), '4AA97269-0FD3-41F1-9B2D-F159C9512324', N'Tư vấn da liễu', N'Tư vấn và điều trị các bệnh về da, dị ứng, mụn.'),
    (NEWID(), 'A5C1984E-6A2B-4DDE-B032-F8DFC036E2C2', N'Khám và điều trị bệnh thần kinh', N'Tư vấn, chẩn đoán và điều trị các bệnh về thần kinh.');

INSERT INTO "Slot" (ID, DoctorID, ServiceID, Title, Description, StartTime, EndTime, Date, SessionFee, Status)
VALUES
    (NEWID(), 'CD94D1AB-6815-4879-8311-EF161C9E4CD7', 'A00A5A7E-D9A3-49D1-8E1E-35FF4CCD55B6', 
        N'Tư vấn tim mạch', N'Tư vấn và kiểm tra các vấn đề về tim mạch', 
        '2025-03-10 08:00:00', '2025-03-10 09:00:00', '2025-03-10', 500000, N'Available'),

    (NEWID(), 'CD94D1AB-6815-4879-8311-EF161C9E4CD7', 'E28AB4E8-912B-46AA-940A-4894A933821D', 
        N'Khám Tai Mũi Họng', N'Chẩn đoán và điều trị các bệnh liên quan đến tai, mũi, họng', 
        '2025-03-11 10:00:00', '2025-03-11 11:00:00', '2025-03-11', 350000, N'Available');

INSERT INTO "AppointmentSlot" (ID, SlotID, ProfileID, Status, CreatedDate, Notes)
VALUES
    (NEWID(), 'D7E27EC6-1063-4E04-9EF2-2FE787E354C8', '15DC99A4-A17F-4AEF-B3AB-AFADBB5EFF69', 
     N'Confirmed', GETDATE(), N'Bệnh nhân cần kiểm tra tim mạch định kỳ.');

INSERT INTO "AppointmentSlotMedicine" (AppointmentSlotMedicineID, AppointmentSlotID, MedicineID, Dosage, Instructions, Quantity)
VALUES
    (NEWID(), 'F645B680-101F-4F55-8262-DC44EFBC280A',
     '8421D78C-0FA1-4F6A-904E-2CC08E81DB2F', N'1 viên/lần', N'Uống sau khi ăn', 10),

    (NEWID(), 'F645B680-101F-4F55-8262-DC44EFBC280A',
     'C374F11E-4D14-4459-900E-85C455BAF3CE', N'2 viên/ngày', N'Uống trước khi ăn sáng', 20),

    (NEWID(), 'F645B680-101F-4F55-8262-DC44EFBC280A',
     '716BA809-369B-44C4-A8BF-93B5633507FA', N'1 gói/ngày', N'Hòa với 100ml nước, uống sau bữa trưa', 15);
