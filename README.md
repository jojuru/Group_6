# Toimintaympäristö
Järjestelmä palvelee asiakkaan Village Newbies Oy mökkivuokraus-, lisäpalvelu- ja majoitusvarausliiketoimintaa, koostaen yhdeksi ohjelmistoksi asiakkuuksien, varausten, palveluiden, raportoinnin ja laskutuksen hallinnan.

# Järjestelmäkuvaus
Järjestelmä on työpöytäsovellus, joka tarjoaa graafisen käyttöliittymän mökkien, palveluiden, varausten ja asiakkuuksien hallintaan.
Järjestelmä raportoi mökkien majoitusvarausten aikavälin sekä ostettujen lisäpalveluiden halutut toimituspäivät graafisesti.
Varausjärjestelmä ja asiakkuustiedot liitetään ohjelmiston laskutushallintaan, jonka kautta lähetetään asiakaskohtaiset laskut valitulla toimitustavalla.
Järjestelmä koostetaan kolmesta kerroksesta:
- Esityskerros
-- Helppokäyttöinen graafinen käyttöliittymä, joka on suunniteltu käytettäväksi tietokoneen työpöydältä.
- Bisnestoiminnallisuus kerros
-- Toteuttaa varausten, asiakastietojen ja palveluiden hallinnan, kuten tilaukset, muokkaukset jne.
-- Hoitaa erilaisten raporttien koostamisen ja esittämisen.
-- Hoitaa laskujen koostamisen ja lähettämisen sekä mahdolliset muokkaukset.
- Tietojenkäsittelykerros
-- SQL tietokanta, joka tallettaa varaustiedot, palvelutiedot, asiakastiedot ja laskutustiedot.
