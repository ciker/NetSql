CREATE TABLE `Account`  (
  `Id` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `UserName` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '用户名',
  `Password` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '密码',
  `Phone` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '手机号',
  `Email` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '邮箱',
  `LastLoginTime` datetime(0) NOT NULL DEFAULT CURRENT_TIMESTAMP(0) COMMENT '最后登录时间',
  `LastLoginIP` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '最后登录IP',
  `Status` tinyint(1) NOT NULL DEFAULT 0 COMMENT '状态：0、未激活 1、正常 2、禁用 3、注销',
  `Sort` int(11) NOT NULL DEFAULT 0 COMMENT '排序',
  `CreatedTime` datetime(0) NOT NULL DEFAULT CURRENT_TIMESTAMP(0) COMMENT '创建时间',
  `Creator` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '创建人',
  `LastModifiedTime` datetime(0) NOT NULL DEFAULT CURRENT_TIMESTAMP(0) COMMENT '最后修改时间',
  `LastModifiedBy` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '最后修改人',
  `ClosedTime` datetime(0) NOT NULL DEFAULT CURRENT_TIMESTAMP(0) COMMENT '注销时间',
  `ClosedBy` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '注销人',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;