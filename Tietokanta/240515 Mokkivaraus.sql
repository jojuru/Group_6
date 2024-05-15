-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------
-- -----------------------------------------------------
-- Schema vn
-- -----------------------------------------------------
DROP SCHEMA IF EXISTS `vn` ;

-- -----------------------------------------------------
-- Schema vn
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `vn` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;

USE `vn` ;

-- -----------------------------------------------------
-- Table `vn`.`alue`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `vn`.`alue` ;

CREATE TABLE IF NOT EXISTS `vn`.`alue` (
  `alue_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `nimi` VARCHAR(40) NULL DEFAULT NULL,
  PRIMARY KEY (`alue_id`),
  INDEX `alue_nimi_index` (`nimi` ASC) VISIBLE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_general_ci;


-- -----------------------------------------------------
-- Table `vn`.`posti`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `vn`.`posti` ;

CREATE TABLE IF NOT EXISTS `vn`.`posti` (
  `postinro` CHAR(5) NOT NULL,
  `toimipaikka` VARCHAR(45) NULL DEFAULT NULL,
  PRIMARY KEY (`postinro`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_general_ci;


-- -----------------------------------------------------
-- Table `vn`.`asiakas`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `vn`.`asiakas` ;

CREATE TABLE IF NOT EXISTS `vn`.`asiakas` (
  `asiakas_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `postinro` CHAR(5) NOT NULL,
  `etunimi` VARCHAR(20) NULL DEFAULT NULL,
  `sukunimi` VARCHAR(40) NULL DEFAULT NULL,
  `lahiosoite` VARCHAR(40) NULL DEFAULT NULL,
  `email` VARCHAR(50) NULL DEFAULT NULL,
  `puhelinnro` VARCHAR(15) NULL DEFAULT NULL,
  PRIMARY KEY (`asiakas_id`),
  INDEX `fk_as_posti1_idx` (`postinro` ASC) VISIBLE,
  INDEX `asiakas_snimi_idx` (`sukunimi` ASC) VISIBLE,
  INDEX `asiakas_enimi_idx` (`etunimi` ASC) VISIBLE,
  CONSTRAINT `fk_asiakas_posti`
    FOREIGN KEY (`postinro`)
    REFERENCES `vn`.`posti` (`postinro`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_general_ci;


-- -----------------------------------------------------
-- Table `vn`.`mokki`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `vn`.`mokki` ;

CREATE TABLE IF NOT EXISTS `vn`.`mokki` (
  `mokki_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `alue_id` INT UNSIGNED NOT NULL,
  `postinro` CHAR(5) NOT NULL,
  `mokkinimi` VARCHAR(45) NULL DEFAULT NULL,
  `katuosoite` VARCHAR(45) NULL DEFAULT NULL,
  `hinta` DOUBLE(8,2) NOT NULL,
  `kuvaus` VARCHAR(150) NULL DEFAULT NULL,
  `henkilomaara` INT NULL DEFAULT NULL,
  `varustelu` VARCHAR(100) NULL DEFAULT NULL,
  `kuva` VARCHAR(100) NULL DEFAULT NULL, -- uusi sarake kuvan tiedostonimelle
  PRIMARY KEY (`mokki_id`),
  INDEX `fk_mokki_alue_idx` (`alue_id` ASC) VISIBLE,
  INDEX `fk_mokki_posti_idx` (`postinro` ASC) VISIBLE,
  CONSTRAINT `fk_mokki_alue`
    FOREIGN KEY (`alue_id`)
    REFERENCES `vn`.`alue` (`alue_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_mokki_posti`
    FOREIGN KEY (`postinro`)
    REFERENCES `vn`.`posti` (`postinro`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_general_ci;


-- -----------------------------------------------------
-- Table `vn`.`varaus`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `vn`.`varaus` ;

CREATE TABLE IF NOT EXISTS `vn`.`varaus` (
  `varaus_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `asiakas_id` INT UNSIGNED NOT NULL,
  `mokki_id` INT UNSIGNED NOT NULL,
  `varattu_pvm` DATETIME NULL DEFAULT NULL,
  `vahvistus_pvm` DATETIME NULL DEFAULT NULL,
  `varattu_alkupvm` DATETIME NULL DEFAULT NULL,
  `varattu_loppupvm` DATETIME NULL DEFAULT NULL,
  PRIMARY KEY (`varaus_id`),
  INDEX `varaus_as_id_index` (`asiakas_id` ASC) VISIBLE,
  INDEX `fk_var_mok_idx` (`mokki_id` ASC) VISIBLE,
  CONSTRAINT `fk_varaus_mokki`
    FOREIGN KEY (`mokki_id`)
    REFERENCES `vn`.`mokki` (`mokki_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `varaus_ibfk`
    FOREIGN KEY (`asiakas_id`)
    REFERENCES `vn`.`asiakas` (`asiakas_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_general_ci;


-- -----------------------------------------------------
-- Table `vn`.`lasku`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `vn`.`lasku` ;

CREATE TABLE IF NOT EXISTS `vn`.`lasku` (
  `lasku_id` INT NOT NULL AUTO_INCREMENT,
  `varaus_id` INT UNSIGNED NOT NULL,
  `summa` DOUBLE(8,2) NOT NULL,
  `alv` DOUBLE(8,2) NOT NULL,
  `maksettu` TINYINT NOT NULL DEFAULT 0,
  `laskun_tyyppi` TINYINT(1) NOT NULL, -- uusi sarake laskun tyypille
  PRIMARY KEY (`lasku_id`),
  INDEX `lasku_ibfk_1` (`varaus_id` ASC) VISIBLE,
  CONSTRAINT `lasku_ibfk_1`
    FOREIGN KEY (`varaus_id`)
    REFERENCES `vn`.`varaus` (`varaus_id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_general_ci;


-- -----------------------------------------------------
-- Table `vn`.`palvelu`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `vn`.`palvelu` ;

CREATE TABLE IF NOT EXISTS `vn`.`palvelu` (
  `palvelu_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `alue_id` INT UNSIGNED NOT NULL,
  `nimi` VARCHAR(40) NULL DEFAULT NULL,
  `kuvaus` VARCHAR(255) NULL DEFAULT NULL,
  `hinta` DOUBLE(8,2) NOT NULL,
  `alv` DOUBLE(8,2) NOT NULL,
  PRIMARY KEY (`palvelu_id`),
  INDEX `Palvelu_nimi_index` (`nimi` ASC) VISIBLE,
  INDEX `palv_toimip_id_ind` (`alue_id` ASC) VISIBLE,
  CONSTRAINT `palvelu_ibfk_1`
    FOREIGN KEY (`alue_id`)
    REFERENCES `vn`.`alue` (`alue_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_general_ci;


-- -----------------------------------------------------
-- Table `vn`.`varauksen_palvelut`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `vn`.`varauksen_palvelut` ;

CREATE TABLE IF NOT EXISTS `vn`.`varauksen_palvelut` (
  `varaus_id` INT UNSIGNED NOT NULL,
  `palvelu_id` INT UNSIGNED NOT NULL,
  `lkm` INT NOT NULL,
  PRIMARY KEY (`palvelu_id`, `varaus_id`),
  INDEX `vp_varaus_id_index` (`varaus_id` ASC) VISIBLE,
  INDEX `vp_palvelu_id_index` (`palvelu_id` ASC) VISIBLE,
  CONSTRAINT `fk_palvelu`
    FOREIGN KEY (`palvelu_id`)
    REFERENCES `vn`.`palvelu` (`palvelu_id`),
  CONSTRAINT `fk_varaus`
    FOREIGN KEY (`varaus_id`)
    REFERENCES `vn`.`varaus` (`varaus_id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_general_ci;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;

------------------------------------------------------------
-- testidatan syöttö, data syötetty riippuvuusjärjestyksessä
------------------------------------------------------------

-- posti -- perään nimetyt alueet mökkien alueita, muut asiakkaiden alueita. 
INSERT INTO `vn`.`posti` (`postinro`, `toimipaikka`) VALUES ('93830', 'Rukatunturi'); -- Ruka
INSERT INTO `vn`.`posti` (`postinro`, `toimipaikka`) VALUES ('73310', 'Tahkovuori'); -- Tahko
INSERT INTO `vn`.`posti` (`postinro`, `toimipaikka`) VALUES ('95980', 'Ylläsjärvi'); -- Ylläs
INSERT INTO `vn`.`posti` (`postinro`, `toimipaikka`) VALUES ('42100', 'Jämsä'); -- Himos
INSERT INTO `vn`.`posti` (`postinro`, `toimipaikka`) VALUES ('99130', 'Sirkka'); -- Levi
INSERT INTO `vn`.`posti` (`postinro`, `toimipaikka`) VALUES ('88610', 'Vuokatti'); -- Vuokatti
INSERT INTO `vn`.`posti` (`postinro`, `toimipaikka`) VALUES ('28900', 'Pori'); -- Pori
INSERT INTO `vn`.`posti` (`postinro`, `toimipaikka`) VALUES ('52200', 'Puumala'); -- Puumala
INSERT INTO `vn`.`posti` (`postinro`, `toimipaikka`) VALUES ('02600', 'Espoo');
INSERT INTO `vn`.`posti` (`postinro`, `toimipaikka`) VALUES ('00420', 'Helsinki');
INSERT INTO `vn`.`posti` (`postinro`, `toimipaikka`) VALUES ('20300', 'Turku');
INSERT INTO `vn`.`posti` (`postinro`, `toimipaikka`) VALUES ('33531', 'Tampere');
INSERT INTO `vn`.`posti` (`postinro`, `toimipaikka`) VALUES ('70210', 'Kuopio');
INSERT INTO `vn`.`posti` (`postinro`, `toimipaikka`) VALUES ('60100', 'Seinäjoki');
INSERT INTO `vn`.`posti` (`postinro`, `toimipaikka`) VALUES ('65101', 'Vaasa');
INSERT INTO `vn`.`posti` (`postinro`, `toimipaikka`) VALUES ('90100', 'Oulu');
INSERT INTO `vn`.`posti` (`postinro`, `toimipaikka`) VALUES ('40100', 'Jyväskylä');
INSERT INTO `vn`.`posti` (`postinro`, `toimipaikka`) VALUES ('28120', 'Pori');



-- alueet -- lomamökkien ja palveluiden alueet nimettyinä. Nyt on olemassa 8 aluetta kuten tehtävässä on haluttu.
INSERT INTO `vn`.`alue` (`alue_id`, `nimi`) VALUES (1, 'Ruka');
INSERT INTO `vn`.`alue` (`alue_id`, `nimi`) VALUES (2, 'Tahko');
INSERT INTO `vn`.`alue` (`alue_id`, `nimi`) VALUES (3, 'Ylläs');
INSERT INTO `vn`.`alue` (`alue_id`, `nimi`) VALUES (4, 'Himos');
INSERT INTO `vn`.`alue` (`alue_id`, `nimi`) VALUES (5, 'Levi');
INSERT INTO `vn`.`alue` (`alue_id`, `nimi`) VALUES (6, 'Vuokatti');
INSERT INTO `vn`.`alue` (`alue_id`, `nimi`) VALUES (7, 'Pori');
INSERT INTO `vn`.`alue` (`alue_id`, `nimi`) VALUES (8, 'Puumala');


-- asiakas
INSERT INTO `vn`.`asiakas` (`asiakas_id`, `postinro`, `etunimi`, `sukunimi`, `lahiosoite`, `email`, `puhelinnro`)
VALUES (1, '02600', 'Matti', 'Meikäläinen', 'Katuosoite 123', 'matti@matinosoite.com', '0401234567');
INSERT INTO `vn`.`asiakas` (`asiakas_id`, `postinro`, `etunimi`, `sukunimi`, `lahiosoite`, `email`, `puhelinnro`)
VALUES (2, '20300', 'Ville', 'Vilhunen', 'Kososenkatu 13', 'vilhusville@firma.com', '0406734863');
INSERT INTO `vn`.`asiakas` (`asiakas_id`, `postinro`, `etunimi`, `sukunimi`, `lahiosoite`, `email`, `puhelinnro`)
VALUES (3, '70210', 'Kaarin', 'Savela', 'Iskurintie 8', 'ksavela@gmail.com', '0408885754');
INSERT INTO `vn`.`asiakas` (`asiakas_id`, `postinro`, `etunimi`, `sukunimi`, `lahiosoite`, `email`, `puhelinnro`)
VALUES (4, '33531', 'Maarit', 'Nokela', 'Kotikatu 6', 'nokimaarit@gmail.com', '0407654321');
INSERT INTO `vn`.`asiakas` (`asiakas_id`, `postinro`, `etunimi`, `sukunimi`, `lahiosoite`, `email`, `puhelinnro`)
VALUES (5, '28120', 'Jutta', 'Kuusi', 'Leivontie 3', 'jutta@moutlook.com', '0402234456');
INSERT INTO `vn`.`asiakas` (`asiakas_id`, `postinro`, `etunimi`, `sukunimi`, `lahiosoite`, `email`, `puhelinnro`)
VALUES (6, '40100', 'Heli', 'Tenhunen', 'Peiponpolku 4', 'heli@firma.com', '0506678890');
INSERT INTO `vn`.`asiakas` (`asiakas_id`, `postinro`, `etunimi`, `sukunimi`, `lahiosoite`, `email`, `puhelinnro`)
VALUES (7, '60100', 'Kari', 'Metsälä', 'Varistie 8', 'kari@gmail.com', '0403335556');
INSERT INTO `vn`.`asiakas` (`asiakas_id`, `postinro`, `etunimi`, `sukunimi`, `lahiosoite`, `email`, `puhelinnro`)
VALUES (8, '90100', 'Outi', 'Latvala', 'Tilhitie 6', 'outi@gmail.com', '0407894321');
INSERT INTO `vn`.`asiakas` (`asiakas_id`, `postinro`, `etunimi`, `sukunimi`, `lahiosoite`, `email`, `puhelinnro`)
VALUES (9, '65101', 'Minna', 'Lehti', 'Sammalpolku 77', 'Minna@moutlook.com', '0402234567');
INSERT INTO `vn`.`asiakas` (`asiakas_id`, `postinro`, `etunimi`, `sukunimi`, `lahiosoite`, `email`, `puhelinnro`)
VALUES (10, '70210', 'Jaakko', 'Metsä', 'Metsätie 888', 'jaakko.metsa@firma.com', '0506674444');
INSERT INTO `vn`.`asiakas` (`asiakas_id`, `postinro`, `etunimi`, `sukunimi`, `lahiosoite`, `email`, `puhelinnro`)
VALUES (11, '02600', 'Sakari', 'Sievinen', 'Perhospolku 987', 'sakari@gmail.com', '0403339990');
INSERT INTO `vn`.`asiakas` (`asiakas_id`, `postinro`, `etunimi`, `sukunimi`, `lahiosoite`, `email`, `puhelinnro`)
VALUES (12, '20300', 'Mervi', 'Herranen', 'Susitie 234', 'mervi@gmail.com', '0407894321');


-- mökki -- perään nimetty selvyyden vuoksi minkä alueen mökki kyseessä, jotta testidatan syöttö on mahd. helppoa
INSERT INTO `vn`.`mokki` (`mokki_id`, `alue_id`, `postinro`, `mokkinimi`, `katuosoite`, `hinta`, `kuvaus`, `henkilomaara`, `varustelu`, `kuva`)
VALUES (1, 1, '93830', 'Rantala', 'Rantatie 12', 134.00, 'Kaunis ja tilava mökki järven rannalla.', 8, 'Sauna, pesukone, takka', 'mokki1.jpg'); -- Ruka
INSERT INTO `vn`.`mokki` (`mokki_id`, `alue_id`, `postinro`, `mokkinimi`, `katuosoite`, `hinta`, `kuvaus`, `henkilomaara`, `varustelu`, `kuva`)
VALUES (2, 2, '73310', 'Metsäpirtti', 'Metsätie 15', 140.00, 'Suuri mökki hissien läheisyydessä.', 6, 'Sauna, tv, pesukone', 'mokki2.jpg'); -- Tahko
INSERT INTO `vn`.`mokki` (`mokki_id`, `alue_id`, `postinro`, `mokkinimi`, `katuosoite`, `hinta`, `kuvaus`, `henkilomaara`, `varustelu`, `kuva`)
VALUES (3, 3, '95980', 'Rauhala', 'Viistotie 24', 120.00, 'Rauhallinen sijainti, juuri remontoitu mökki, palveluiden ääressä.', 5, 'Sauna, astianpesukone', 'mokki3.jpg'); -- Ylläs
INSERT INTO `vn`.`mokki` (`mokki_id`, `alue_id`, `postinro`, `mokkinimi`, `katuosoite`, `hinta`, `kuvaus`, `henkilomaara`, `varustelu`, `kuva`)
VALUES (4, 4, '42100', 'El Massimo', 'Massitie 1', 200.00, 'Täysvarusteltu suuri luksusmökki järven rannalla omalla paljulla.', 10, 'Sauna, tv, astianpesukone, pyykinpesukone, takka', 'mokki4.jpg');  -- Himos
INSERT INTO `vn`.`mokki` (`mokki_id`, `alue_id`, `postinro`, `mokkinimi`, `katuosoite`, `hinta`, `kuvaus`, `henkilomaara`, `varustelu`, `kuva`)
VALUES (5, 5, '99130', 'Tuohitossu', 'Järvipolku 48', 90.00, 'Kaunis, rauhallinen ja pieni mökki.', 3, 'Sauna, takka', 'mokki5.jpg');  -- Levi
INSERT INTO `vn`.`mokki` (`mokki_id`, `alue_id`, `postinro`, `mokkinimi`, `katuosoite`, `hinta`, `kuvaus`, `henkilomaara`, `varustelu`, `kuva`)
VALUES (6, 6, '88610', 'Virsu', 'Järvipolku 175', 100.00, 'Tilava perusmökki järvimaiseman ystäville.', 4, 'Sauna, takka, pesukone, tv', 'mokki6.jpg');  -- Vuokatti
INSERT INTO `vn`.`mokki` (`mokki_id`, `alue_id`, `postinro`, `mokkinimi`, `katuosoite`, `hinta`, `kuvaus`, `henkilomaara`, `varustelu`, `kuva`)
VALUES (7, 7, '28900', 'Erä-Jooseppi', 'Kullanvuolijantie 76', 70.00, 'Kaunis mökki järven rannalla.', 2, 'sauna', 'mokki7.jpg'); -- Pori
INSERT INTO `vn`.`mokki` (`mokki_id`, `alue_id`, `postinro`, `mokkinimi`, `katuosoite`, `hinta`, `kuvaus`, `henkilomaara`, `varustelu`, `kuva`)
VALUES (8, 8, '52200', 'Kultahippu', 'Vaskoolintie 6', 180.00, 'Rauhallinen metsämaisema, soliseva tunturipuro ja rauha.', 5, 'sauna, takka, pesukone', 'mokki8.jpg'); -- Puumala
INSERT INTO `vn`.`mokki` (`mokki_id`, `alue_id`, `postinro`, `mokkinimi`, `katuosoite`, `hinta`, `kuvaus`, `henkilomaara`, `varustelu`, `kuva`)
VALUES (9, 2, '73310', 'Metsälä', 'Rinnetie 77', 175.00, 'Laskettelijan unelmamökki rinteiden välittömässä läheisyydessä näkymällä rinteille!', 5, 'takka, astianpesukone, pyykinpesukone', 'mokki1.jpg'); -- Tahko
INSERT INTO `vn`.`mokki` (`mokki_id`, `alue_id`, `postinro`, `mokkinimi`, `katuosoite`, `hinta`, `kuvaus`, `henkilomaara`, `varustelu`, `kuva`)
VALUES (10, 1, '93830', 'Rinnetorppa', 'Rantaraitti 2', 123.00, 'Upea lomaparatiisi rauhaa rakastaville.', 8, 'Sauna, pesukone, takka', 'mokki2.jpg'); -- Ruka
INSERT INTO `vn`.`mokki` (`mokki_id`, `alue_id`, `postinro`, `mokkinimi`, `katuosoite`, `hinta`, `kuvaus`, `henkilomaara`, `varustelu`, `kuva`)
VALUES (11, 2, '73310', 'Metsäpirtti', 'Metsäpoluntie 34', 120.00, 'Iso, tilava mökki lähellä palveluita.', 6, 'Sauna, tv, pesukone', 'mokki3.jpg'); -- Tahko
INSERT INTO `vn`.`mokki` (`mokki_id`, `alue_id`, `postinro`, `mokkinimi`, `katuosoite`, `hinta`, `kuvaus`, `henkilomaara`, `varustelu`, `kuva`)
VALUES (12, 3, '95980', 'Paratiisi', 'Pellonpääntie 14', 110.00, 'Äskettäin remontoitu mökki rauhallisella sijainnilla.', 5, 'Sauna, astianpesukone', 'mokki4.jpg'); -- Ylläs
INSERT INTO `vn`.`mokki` (`mokki_id`, `alue_id`, `postinro`, `mokkinimi`, `katuosoite`, `hinta`, `kuvaus`, `henkilomaara`, `varustelu`, `kuva`)
VALUES (13, 4, '42100', 'Unelma', 'Ukontie 2', 180.00, 'Ulkoporealtaalla varusteltu tilava mökki.', 10, 'Sauna, tv, astianpesukone, pyykinpesukone, takka', 'mokki5.jpg');  -- Himos
INSERT INTO `vn`.`mokki` (`mokki_id`, `alue_id`, `postinro`, `mokkinimi`, `katuosoite`, `hinta`, `kuvaus`, `henkilomaara`, `varustelu`, `kuva`)
VALUES (14, 5, '99130', 'Hirsipirtti', 'Akantie 1', 80.00, 'Kaunis, rauhallinen mökki lähellä palveluita.', 3, 'Sauna, takka', 'mokki6.jpg');  -- Levi
INSERT INTO `vn`.`mokki` (`mokki_id`, `alue_id`, `postinro`, `mokkinimi`, `katuosoite`, `hinta`, `kuvaus`, `henkilomaara`, `varustelu`, `kuva`)
VALUES (15, 6, '88610', 'Sammalmökki', 'Saarelanpolku 568', 95.00, 'Tilava perusmökki lähellä rinteitä.', 4, 'Sauna, takka, pesukone, tv', 'mokki7.jpg');  -- Vuokatti
INSERT INTO `vn`.`mokki` (`mokki_id`, `alue_id`, `postinro`, `mokkinimi`, `katuosoite`, `hinta`, `kuvaus`, `henkilomaara`, `varustelu`, `kuva`)
VALUES (16, 7, '28900', 'Kivelä', 'Kalliolohkontie 21', 120.00, 'Toimiva perusmökki isommankin porukan tarpeisiin..', 2, 'sauna, tv', 'mokki8.jpg'); -- Pori
INSERT INTO `vn`.`mokki` (`mokki_id`, `alue_id`, `postinro`, `mokkinimi`, `katuosoite`, `hinta`, `kuvaus`, `henkilomaara`, `varustelu`, `kuva`)
VALUES (17, 8, '52200', 'Saarela', 'Saarelanpolku 7', 190.00, 'Luontoa rakastavien valinta lähellä patikointimaastoja.', 5, 'sauna, takka, pesukone', 'mokki1.jpg'); -- Puumala
INSERT INTO `vn`.`mokki` (`mokki_id`, `alue_id`, `postinro`, `mokkinimi`, `katuosoite`, `hinta`, `kuvaus`, `henkilomaara`, `varustelu`, `kuva`)
VALUES (18, 2, '73310', 'Erämaan kutsu', 'Kattilamäentie 890', 165.00, 'Laskettelijan unelmamökki rinteiden välittömässä läheisyydessä.', 5, 'takka, tv, astianpesukone, pyykinpesukone', 'mokki2.jpg'); -- Tahko


-- varaus -- muutama kuvitteellinen varaus
INSERT INTO `vn`.`varaus` (`varaus_id`, `asiakas_id`, `mokki_id`, `varattu_pvm`, `vahvistus_pvm`, `varattu_alkupvm`, `varattu_loppupvm`)
VALUES (1, 1, 1, '2023-05-10 14:00:00', '2023-05-02 14:00:00', '2023-06-01 00:00:00', '2023-06-07 00:00:00');
INSERT INTO `vn`.`varaus` (`varaus_id`, `asiakas_id`, `mokki_id`, `varattu_pvm`, `vahvistus_pvm`, `varattu_alkupvm`, `varattu_loppupvm`)
VALUES (2, 2, 2, '2023-10-12 11:32:44', '2023-10-12 11:35:12', '2023-11-11 00:00:00', '2023-11-18 00:00:00');
INSERT INTO `vn`.`varaus` (`varaus_id`, `asiakas_id`, `mokki_id`, `varattu_pvm`, `vahvistus_pvm`, `varattu_alkupvm`, `varattu_loppupvm`)
VALUES (3, 3, 3, '2023-04-01 18:00:34', '2023-04-01 18:05:14', '2023-12-6 00:00:00', '2023-12-9 00:00:00');
INSERT INTO `vn`.`varaus` (`varaus_id`, `asiakas_id`, `mokki_id`, `varattu_pvm`, `vahvistus_pvm`, `varattu_alkupvm`, `varattu_loppupvm`)
VALUES (4, 2, 3, '2023-04-02 12:12:12', '2023-04-02 12:13:14', '2023-12-10 00:00:00', '2023-12-17 00:00:00');
INSERT INTO `vn`.`varaus` (`varaus_id`, `asiakas_id`, `mokki_id`, `varattu_pvm`, `vahvistus_pvm`, `varattu_alkupvm`, `varattu_loppupvm`)
VALUES (5, 3, 5, '2023-05-02 12:01:12', '2023-05-03 08:01:14', '2023-06-10 00:00:00', '2023-06-24 00:00:00');
INSERT INTO `vn`.`varaus` (`varaus_id`, `asiakas_id`, `mokki_id`, `varattu_pvm`, `vahvistus_pvm`, `varattu_alkupvm`, `varattu_loppupvm`)
VALUES (6, 2, 6, '2023-07-01 09:09:09', '2023-07-03 09:09:14', '2023-06-10 00:00:00', '2023-06-17 00:00:00');
INSERT INTO `vn`.`varaus` (`varaus_id`, `asiakas_id`, `mokki_id`, `varattu_pvm`, `vahvistus_pvm`, `varattu_alkupvm`, `varattu_loppupvm`)
VALUES (7, 4, 7, '2023-05-10 14:00:00', '2023-05-02 14:00:00', '2023-06-01 00:00:00', '2023-06-07 00:00:00');
INSERT INTO `vn`.`varaus` (`varaus_id`, `asiakas_id`, `mokki_id`, `varattu_pvm`, `vahvistus_pvm`, `varattu_alkupvm`, `varattu_loppupvm`)
VALUES (8, 5, 8, '2023-10-12 11:32:44', '2023-10-12 11:35:12', '2023-11-11 00:00:00', '2023-11-18 00:00:00');
INSERT INTO `vn`.`varaus` (`varaus_id`, `asiakas_id`, `mokki_id`, `varattu_pvm`, `vahvistus_pvm`, `varattu_alkupvm`, `varattu_loppupvm`)
VALUES (9, 6, 9, '2023-04-01 18:00:34', '2023-04-01 18:05:14', '2023-12-6 00:00:00', '2023-12-9 00:00:00');
INSERT INTO `vn`.`varaus` (`varaus_id`, `asiakas_id`, `mokki_id`, `varattu_pvm`, `vahvistus_pvm`, `varattu_alkupvm`, `varattu_loppupvm`)
VALUES (10, 7, 10, '2023-04-02 12:12:12', '2023-04-02 12:13:14', '2023-12-10 00:00:00', '2023-12-17 00:00:00');
INSERT INTO `vn`.`varaus` (`varaus_id`, `asiakas_id`, `mokki_id`, `varattu_pvm`, `vahvistus_pvm`, `varattu_alkupvm`, `varattu_loppupvm`)
VALUES (11, 8, 11, '2023-05-02 12:01:12', '2023-05-03 08:01:14', '2023-06-10 00:00:00', '2023-06-24 00:00:00');
INSERT INTO `vn`.`varaus` (`varaus_id`, `asiakas_id`, `mokki_id`, `varattu_pvm`, `vahvistus_pvm`, `varattu_alkupvm`, `varattu_loppupvm`)
VALUES (12, 9, 12, '2023-07-01 09:09:09', '2023-07-03 09:09:14', '2023-06-10 00:00:00', '2023-06-17 00:00:00');
INSERT INTO `vn`.`varaus` (`varaus_id`, `asiakas_id`, `mokki_id`, `varattu_pvm`, `vahvistus_pvm`, `varattu_alkupvm`, `varattu_loppupvm`)
VALUES (13, 10, 13, '2023-07-01 09:09:09', '2023-07-03 09:09:14', '2023-06-10 00:00:00', '2023-06-17 00:00:00');
INSERT INTO `vn`.`varaus` (`varaus_id`, `asiakas_id`, `mokki_id`, `varattu_pvm`, `vahvistus_pvm`, `varattu_alkupvm`, `varattu_loppupvm`)
VALUES (14, 11, 14, '2023-07-01 09:09:09', '2023-07-03 09:09:14', '2023-06-10 00:00:00', '2023-06-17 00:00:00');
INSERT INTO `vn`.`varaus` (`varaus_id`, `asiakas_id`, `mokki_id`, `varattu_pvm`, `vahvistus_pvm`, `varattu_alkupvm`, `varattu_loppupvm`)
VALUES (15, 12, 15, '2023-07-01 09:09:09', '2023-07-03 09:09:14', '2023-06-10 00:00:00', '2023-06-17 00:00:00');
INSERT INTO `vn`.`varaus` (`varaus_id`, `asiakas_id`, `mokki_id`, `varattu_pvm`, `vahvistus_pvm`, `varattu_alkupvm`, `varattu_loppupvm`)
VALUES (16, 7, 16, '2023-07-01 09:09:09', '2023-07-03 09:09:14', '2023-06-10 00:00:00', '2023-06-17 00:00:00');
INSERT INTO `vn`.`varaus` (`varaus_id`, `asiakas_id`, `mokki_id`, `varattu_pvm`, `vahvistus_pvm`, `varattu_alkupvm`, `varattu_loppupvm`)
VALUES (17, 8, 17, '2023-07-01 09:09:09', '2023-07-03 09:09:14', '2023-06-10 00:00:00', '2023-06-17 00:00:00');
INSERT INTO `vn`.`varaus` (`varaus_id`, `asiakas_id`, `mokki_id`, `varattu_pvm`, `vahvistus_pvm`, `varattu_alkupvm`, `varattu_loppupvm`)
VALUES (18, 9, 18, '2023-07-01 09:09:09', '2023-07-03 09:09:14', '2023-06-10 00:00:00', '2023-06-17 00:00:00');


-- laskut
INSERT INTO `vn`.`lasku` (`lasku_id`, `varaus_id`, `summa`, `alv`, `maksettu`, `laskun_tyyppi`)
VALUES 
(1, 1, 804.00, 155.61, 1, 0),
(2, 2, 980.00, 189.68, 0, 1),
(3, 3, 360.00, 69.68, 1, 0),
(4, 4, 840.00, 162.58, 0, 1),
(5, 5, 1260.00, 243.87, 0, 0),
(6, 6, 700.00, 135.48, 1, 1);

-- palvelut
INSERT INTO `vn`.`palvelu` (`palvelu_id`,`alue_id`, `nimi`, `kuvaus`, `hinta`, `alv`) VALUES 
(101, 1, 'Porosafari', 'Safari with reindeer', 120.00, 28.80),
(102, 2, 'Porosafari', 'Safari with reindeer', 120.00, 28.80),
(103, 3, 'Porosafari', 'Safari with reindeer', 120.00, 28.80),
(104, 1, 'Koiravaljakkoajelu', 'Dog sled ride through snowy landscapes', 90.00, 21.60),
(105, 2, 'Koiravaljakkoajelu', 'Dog sled ride through snowy landscapes', 90.00, 21.60),
(106, 3, 'Koiravaljakkoajelu', 'Dog sled ride through snowy landscapes', 90.00, 21.60),
(107, 1, 'Hevosajelu', 'Horseback riding through scenic trails', 75.00, 18.00),
(108, 2, 'Hevosajelu', 'Horseback riding through scenic trails', 75.00, 18.00),
(109, 3, 'Hevosajelu', 'Horseback riding through scenic trails', 75.00, 18.00),
(110, 4, 'Hevosajelu', 'Horseback riding through scenic trails', 75.00, 18.00),
(111, 4, 'Vesiskootteriajelu', 'Jet ski ride on the lake', 85.00, 20.40),
(112, 5, 'Vesiskootteriajelu', 'Jet ski ride on the lake', 85.00, 20.40),
(113, 8, 'Vesiskootteriajelu', 'Jet ski ride on the lake', 85.00, 20.40),
(114, 4, 'Airsoftaus', 'Tactical airsoft games in a forested area', 65.00, 15.60),
(115, 6, 'Vesiskootteriajelu', 'Jet ski ride on the lake', 85.00, 20.40),
(116, 6, 'Airsoftaus', 'Tactical airsoft games in a forested area', 65.00, 15.60),
(117, 7, 'Airsoftaus', 'Tactical airsoft games in a forested area', 65.00, 15.60),
(118, 8, 'Airsoftaus', 'Tactical airsoft games in a forested area', 65.00, 15.60);


-- varauksen palvelut
INSERT INTO `vn`.`varauksen_palvelut` (`varaus_id`, `palvelu_id`, `lkm`)
VALUES (1, 101, 1),
(2, 102, 1),
(3, 103, 1),
(4, 106, 1),
(5, 112, 1),
(6, 116, 1),
(1, 104, 1),
(3, 109, 1),
(6, 115, 1);